using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DragWithButtons : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragRange = 5f;
    public float dragSpeed = 5f;
    public float rotationSpeed = 90f;
    public float objectFollowSpeed = 10f;

    [Header("Visual Feedback")]
    public LineRenderer pointerLine;
    public Transform handTransform;

    [Header("Laser Settings")]
    public Color defaultLaserColor = Color.white;
    public Color hoverLaserColor = Color.green;
    public Color rotatingLaserColor = Color.blue;
    public float laserWidth = 0.005f;

    [Header("Input Actions")]
    public InputActionProperty grabAction;    // Trigger for grabbing
    public InputActionProperty moveAction;    // Thumbstick for moving
    public InputActionProperty rotateAction;  // Secondary button for rotation

    [Header("UI Interaction")]
    public LayerMask uiLayerMask = 1 << 5;   // UI layer
    public LayerMask dragLayerMask;           // What objects can be dragged

    private GameObject draggedObject;
    private Rigidbody draggedRb;
    private Vector3 grabOffset;
    private float currentDragDistance;
    private bool isRotating = false;

    void Start()
    {
        // Set default drag layer mask if not assigned (everything except UI and Player)
        if (dragLayerMask.value == 0)
            dragLayerMask = ~((1 << 2) | (1 << 5)); // Ignore IgnoreRaycast and UI layers

        // Initialize LineRenderer
        InitializeLineRenderer();

        // Enable input actions
        grabAction.action.Enable();
        moveAction.action.Enable();
        if (rotateAction.action != null)
            rotateAction.action.Enable();
    }

    void InitializeLineRenderer()
    {
        if (pointerLine != null)
        {
            pointerLine.positionCount = 2;
            pointerLine.useWorldSpace = true;
            pointerLine.startWidth = laserWidth;
            pointerLine.endWidth = laserWidth;
            pointerLine.startColor = defaultLaserColor;
            pointerLine.endColor = defaultLaserColor;
            pointerLine.enabled = true; // Ensure it's enabled

            Debug.Log("LineRenderer initialized");
        }
        else
        {
            Debug.LogError("LineRenderer not assigned!");
        }
    }

    void OnEnable()
    {
        grabAction.action.performed += OnGrabPerformed;
        grabAction.action.canceled += OnGrabCanceled;
    }

    void OnDisable()
    {
        grabAction.action.performed -= OnGrabPerformed;
        grabAction.action.canceled -= OnGrabCanceled;
        ReleaseDrag();
    }

    void Update()
    {
        UpdatePointer();

        if (draggedObject != null)
        {
            UpdateDraggedObject();
            HandleObjectMovement();
            HandleObjectRotation();
        }
    }

    void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (draggedObject == null)
            TryGrab();
    }

    void OnGrabCanceled(InputAction.CallbackContext context)
    {
        if (draggedObject != null)
            ReleaseDrag();
    }

    void TryGrab()
    {
        // Check if we're pointing at UI (don't grab if we are)
        if (IsPointingAtUI())
            return;

        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange, dragLayerMask))
        {
            Rigidbody hitRb = hit.collider.attachedRigidbody;
            if (hitRb != null && !hitRb.isKinematic)
            {
                draggedObject = hit.collider.gameObject;
                draggedRb = hitRb;

                // Store initial state
                grabOffset = draggedObject.transform.position - hit.point;
                currentDragDistance = hit.distance;

                // Make object easier to move
                draggedRb.useGravity = false;
                draggedRb.linearDamping = 10f; // Increased drag for smoother movement
                draggedRb.angularDamping = 5f;

                Debug.Log($"Grabbed: {draggedObject.name}");
            }
        }
    }

    void ReleaseDrag()
    {
        if (draggedObject == null) return;

        Debug.Log($"Released: {draggedObject.name}");

        // Restore physics
        if (draggedRb != null)
        {
            draggedRb.useGravity = true;
            draggedRb.linearDamping = 0f;
            draggedRb.angularDamping = 0.05f;
        }

        draggedObject = null;
        draggedRb = null;
        isRotating = false;
    }

    void UpdateDraggedObject()
    {
        if (draggedObject == null) return;

        // Calculate target position based on hand position and current drag distance
        Vector3 targetPosition = handTransform.position + handTransform.forward * currentDragDistance + grabOffset;

        // Smoothly move the object
        draggedRb.MovePosition(Vector3.Lerp(draggedObject.transform.position, targetPosition, objectFollowSpeed * Time.deltaTime));
    }

    void HandleObjectMovement()
    {
        // Use thumbstick to adjust distance from controller
        Vector2 thumbstickInput = moveAction.action.ReadValue<Vector2>();

        // Vertical input moves object closer/further
        float distanceChange = thumbstickInput.y * dragSpeed * Time.deltaTime;
        currentDragDistance = Mathf.Clamp(currentDragDistance + distanceChange, 0.5f, dragRange);

        // Horizontal input could be used for something else, like strafing
        // or we can ignore it for now
    }

    void HandleObjectRotation()
    {
        // Use secondary button (usually X or A) to toggle rotation mode
        if (rotateAction.action != null && rotateAction.action.WasPressedThisFrame())
        {
            isRotating = !isRotating;
        }

        if (isRotating)
        {
            Vector2 thumbstickInput = moveAction.action.ReadValue<Vector2>();

            // Use horizontal input for rotation when in rotation mode
            float rotationAmount = thumbstickInput.x * rotationSpeed * Time.deltaTime;
            draggedObject.transform.Rotate(0, rotationAmount, 0, Space.World);

            // Visual feedback for rotation mode (change laser color)
            if (pointerLine != null)
            {
                pointerLine.startColor = rotatingLaserColor;
                pointerLine.endColor = rotatingLaserColor;
            }
        }
        else if (pointerLine != null && draggedObject == null)
        {
            // Reset laser color when not rotating and not dragging
            pointerLine.startColor = defaultLaserColor;
            pointerLine.endColor = defaultLaserColor;
        }
    }

    void UpdatePointer()
    {
        if (!pointerLine || !pointerLine.enabled)
        {
            return;
        }

        Ray ray = new Ray(handTransform.position, handTransform.forward);
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit, dragRange, dragLayerMask);

        // Update line positions
        pointerLine.SetPosition(0, handTransform.position);
        pointerLine.SetPosition(1, hitSomething ? hit.point : handTransform.position + handTransform.forward * dragRange);

        // Update line color based on state
        if (isRotating && draggedObject != null)
        {
            pointerLine.startColor = rotatingLaserColor;
            pointerLine.endColor = rotatingLaserColor;
        }
        else if (hitSomething && hit.collider.attachedRigidbody != null && !hit.collider.attachedRigidbody.isKinematic)
        {
            pointerLine.startColor = hoverLaserColor;
            pointerLine.endColor = hoverLaserColor;
        }
        else
        {
            pointerLine.startColor = defaultLaserColor;
            pointerLine.endColor = defaultLaserColor;
        }
    }

    bool IsPointingAtUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange, uiLayerMask))
        {
            // If we hit UI, don't allow grabbing
            return true;
        }
        return false;
    }

    // Debug method to check LineRenderer visibility
    void CheckLineRendererVisibility()
    {
        if (pointerLine == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
            return;
        }

        Debug.Log($"LineRenderer enabled: {pointerLine.enabled}");
        Debug.Log($"LineRenderer positions: {pointerLine.positionCount}");
        Debug.Log($"LineRenderer material: {pointerLine.material}");
        Debug.Log($"LineRenderer width: {pointerLine.startWidth}");
    }

    // Draw debug ray in Scene view
    void OnDrawGizmos()
    {
        if (handTransform == null) return;

        // Draw debug ray in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawRay(handTransform.position, handTransform.forward * dragRange);

        if (pointerLine != null && pointerLine.enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointerLine.GetPosition(0), pointerLine.GetPosition(1));
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class DragWithButtonsDebug : MonoBehaviour
{
    public float dragRange = 5f;
    public LineRenderer pointerLine;
    public Transform handTransform;
    public InputActionProperty dragButton;

    private GameObject draggedObject;
    private Rigidbody draggedRb;

    void Start()
    {
        dragButton.action.Enable();

        if (pointerLine != null)
        {
            pointerLine.positionCount = 2;
            pointerLine.useWorldSpace = true;
        }
    }

    void Update()
    {
        UpdatePointer();

        bool buttonPressed = dragButton.action.ReadValue<float>() > 0.5f;

        if (buttonPressed && draggedObject == null)
            TryDrag();
        else if (!buttonPressed && draggedObject != null)
            ReleaseDrag();

        if (draggedObject != null)
            UpdateDraggedObject();
    }

    void TryDrag()
    {
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange))
        {
            if (hit.collider.attachedRigidbody != null && !hit.collider.attachedRigidbody.isKinematic)
            {
                draggedObject = hit.collider.gameObject;
                draggedRb = hit.collider.attachedRigidbody;
                draggedRb.useGravity = false;
                draggedRb.isKinematic = true;
            }
        }
    }

    void ReleaseDrag()
    {
        if (draggedObject != null)
        {
            draggedRb.useGravity = true;
            draggedRb.isKinematic = false;
            draggedObject = null;
            draggedRb = null;
        }
    }

    void UpdateDraggedObject()
    {
        Vector3 targetPosition = handTransform.position + handTransform.forward * 1f; // Increased distance
        draggedObject.transform.position = Vector3.Lerp(draggedObject.transform.position, targetPosition, Time.deltaTime * 10f);
    }

    void UpdatePointer()
    {
        if (!pointerLine) return;

        pointerLine.SetPosition(0, handTransform.position);
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange))
            pointerLine.SetPosition(1, hit.point);
        else
            pointerLine.SetPosition(1, handTransform.position + handTransform.forward * dragRange);
    }
}

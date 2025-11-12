using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DragWithTrigger : MonoBehaviour
{
    public float dragRange = 5f;
    public LineRenderer pointerLine;
    public Transform handTransform;
    public InputActionProperty triggerAction;

    private GameObject draggedObject;
    private Rigidbody draggedRb;

    void Update()
    {
        UpdatePointer();

        bool triggerPressed = triggerAction.action.ReadValue<float>() > 0.5f;

        if (triggerPressed && draggedObject == null)
        {
            TryDrag();
        }
        else if (!triggerPressed && draggedObject != null)
        {
            ReleaseDrag();
        }

        if (draggedObject != null)
        {
            UpdateDraggedObject();
        }
    }

    void TryDrag()
    {
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange))
        {
            if (hit.collider.attachedRigidbody)
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
        draggedRb.useGravity = true;
        draggedRb.isKinematic = false;
        draggedObject = null;
        draggedRb = null;
    }

    void UpdateDraggedObject()
    {
        draggedObject.transform.position = Vector3.Lerp(draggedObject.transform.position, handTransform.position + handTransform.forward * 0.5f, Time.deltaTime * 10f);
    }

    void UpdatePointer()
    {
        if (!pointerLine) return;
        pointerLine.SetPosition(0, handTransform.position);
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, dragRange))
        {
            pointerLine.SetPosition(1, hit.point);
        }
        else
        {
            pointerLine.SetPosition(1, handTransform.position + handTransform.forward * dragRange);
        }

    }
}



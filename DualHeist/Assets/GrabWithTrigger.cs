using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabWithTrigger : MonoBehaviour
{
    public float grabRange = 5f;
    public LineRenderer pointerLine;
    public Transform handTransform;
    public InputActionProperty triggerAction;

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Update()
    {
        UpdatePointer();

        bool triggerPressed = triggerAction.action.ReadValue<float>() > 0.5f;

        if (triggerPressed && heldObject == null)
            TryGrab();
        else if (!triggerPressed && heldObject != null)
            ReleaseObject();
    }

    void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, grabRange))
        {
            if (hit.collider.attachedRigidbody)
            {
                heldObject = hit.collider.gameObject;
                heldRb = hit.collider.attachedRigidbody;
                heldRb.useGravity = false;
                heldRb.isKinematic = true;
                heldObject.transform.SetParent(handTransform);
            }
        }
    }

    void ReleaseObject()
    {
        heldObject.transform.SetParent(null);
        heldRb.useGravity = true;
        heldRb.isKinematic = false;
        heldObject = null;
        heldRb = null;
    }

    void UpdatePointer()
    {
        if (!pointerLine) return;
        pointerLine.SetPosition(0, handTransform.position);
        RaycastHit hit;
        if (Physics.Raycast(handTransform.position, handTransform.forward, out hit, grabRange))
            pointerLine.SetPosition(1, hit.point);
        else
            pointerLine.SetPosition(1, handTransform.position + handTransform.forward * grabRange);
    }
}

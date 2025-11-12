using UnityEngine;
using UnityEngine.InputSystem;

public class GrabWithButtons : MonoBehaviour
{
    public float grabRange = 5f;
    public Transform handTransform;
    public InputActionProperty grabButton;

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Update()
    {

        bool buttonPressed = grabButton.action.ReadValue<float>() > 0.5f;

        if (buttonPressed && heldObject == null)
        {
            TryGrab();
        }
        else if (!buttonPressed && heldObject != null)
        {
            ReleaseObject();
        }
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

}
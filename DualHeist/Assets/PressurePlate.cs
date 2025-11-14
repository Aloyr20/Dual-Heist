using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject block;
    [SerializeField] GameObject block1;
    [SerializeField] GameObject hitbox;
    [SerializeField] GameObject hitbox1;
    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("block");
        if (other.gameObject.CompareTag("Box"))
        {
            block.SetActive(true);
            block1.SetActive(false);
            hitbox.SetActive(false);
            hitbox1.SetActive(true);
        }
    }
    public void OnCollisionExit(Collision other)
    {
        Debug.Log("block1");
        if (other.gameObject.CompareTag("Box"))
        {
            block.SetActive(false);
            block1.SetActive(true);
            hitbox.SetActive(true);
            hitbox1.SetActive(false);
        }
    }
}

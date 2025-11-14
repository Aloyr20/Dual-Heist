using UnityEngine;

public class Generator1 : MonoBehaviour
{
    [SerializeField] GameObject block;


    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("aug");
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            gameObject.SetActive(false);
            block.SetActive(false);
        }
    }
}

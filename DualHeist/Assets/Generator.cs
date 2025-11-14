using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] GameObject hitbox;
    [SerializeField] GameObject bridge;


    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("quinn");
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            gameObject.SetActive(false);
            hitbox.SetActive(false);
            bridge.SetActive(true);
        }
    }
}

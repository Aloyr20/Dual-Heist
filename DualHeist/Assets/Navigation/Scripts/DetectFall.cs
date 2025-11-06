using UnityEngine;

public class DetectFall : MonoBehaviour
{
    [SerializeField] private float m_VoidHeight;

    void Update()
    {
        if (transform.position.y < m_VoidHeight)
        {
            Debug.Log("Player has fallen!"); // TO-DO: game over function
        }
    }
}

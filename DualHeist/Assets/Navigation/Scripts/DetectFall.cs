using UnityEngine;
using UnityEngine.AI;

public class DetectFall : MonoBehaviour
{
    [SerializeField] private float m_VoidHeight;

    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private SimpleNavMesh m_Controller;
    [SerializeField] public GameObject m_lose;

    private void Start()
    {
        if (m_Agent == null)
            m_Agent = GetComponent<NavMeshAgent>();
        if (m_Rigidbody == null)
            m_Rigidbody = GetComponent<Rigidbody>();

        m_Rigidbody.useGravity = false;
        m_Rigidbody.isKinematic = false;
    }

    void Update()
    {
        if (transform.position.y < m_VoidHeight)
        {
            Debug.Log("Player has fallen!"); // TO-DO: game over function
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Feller"))
        {
            m_Controller.StopAllCoroutines();
            m_Controller.enabled = false;
            m_Agent.enabled = false;
            m_Rigidbody.useGravity = true;
            m_lose.SetActive(true);
        }
    }
}

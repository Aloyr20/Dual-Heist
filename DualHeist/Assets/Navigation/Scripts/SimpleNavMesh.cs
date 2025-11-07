using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMesh : MonoBehaviour
{
    public GameObject target;

    [SerializeField] private NavMeshAgent m_Agent;

    private void Start()
    {

        // TO-DO: Debug messages, should be removed before building
        if (target == null)
        {
            Debug.LogWarning("Nav Mesh Target is not set in inspector.");
            target = GameObject.FindGameObjectWithTag("Target");
        }
        if (m_Agent == null)
        {
            Debug.LogWarning("Agent is not set in inspector.");
            m_Agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        SendToTarget(); // TO-DO: Debug line. Should be removed when SendToTarget() is rigged properly
    }

    public void SendToTarget()
    {
        m_Agent.SetDestination(target.transform.position);
    }
}

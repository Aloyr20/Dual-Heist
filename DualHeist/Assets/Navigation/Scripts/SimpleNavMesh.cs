using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMesh : MonoBehaviour
{
    public GameObject[] targets;

    [SerializeField] private NavMeshAgent m_Agent;

    /**
     * The script currently sends the agent to the next target one second
     * after reaching the main target. This can be easily tweaked if need
     * be.
     */
    private void Start()
    {
        // TO-DO: Debug messages, should be removed before building
        if (targets == null)
        {
            Debug.LogWarning("Nav Mesh Target is not set in inspector.");
            targets = GameObject.FindGameObjectsWithTag("Target");
        }
        if (m_Agent == null)
        {
            Debug.LogWarning("Agent is not set in inspector.");
            m_Agent = GetComponent<NavMeshAgent>();
        }

        m_Agent.SetDestination(targets[0].transform.position);
    }

    private void Update()
    {
        if (m_Agent.remainingDistance < 0.05f)
            Invoke(nameof(SendToNextTarget), 1f);
    }

    int currentTarget = 0;
    public void SendToNextTarget()
    {
        currentTarget++;
        m_Agent.SetDestination(targets[currentTarget].transform.position);
    }
}

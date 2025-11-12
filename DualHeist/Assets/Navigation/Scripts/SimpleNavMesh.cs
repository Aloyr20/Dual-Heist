using System.Collections;
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

        StartCoroutine(StartMoving()); // TO-DO: Should only run on player input
    }

    private void Update()
    {
    /**
     *  if (input) 
     *  {
     *      StartCoroutine(StartMoving());
     *  }
     */
    }

    public IEnumerator StartMoving()
    {
        int currentTarget = 0;

        SendToTarget(0);
        while (currentTarget < targets.Length)
        {
            if (m_Agent.remainingDistance < 0.01f)
            {
                currentTarget++;
                SendToTarget(currentTarget);
            }

            yield return null;
        }
    }

    public void SendToTarget(int index)
    {
        m_Agent.SetDestination(targets[index].transform.position);
    }
}

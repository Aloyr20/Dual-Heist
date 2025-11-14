using UnityEngine;
using UnityEngine.AI;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private bool m_EnableDebug;
    [SerializeField] private GameObject m_Player;

    [SerializeField] private MeshFilter m_PlayerMesh;
    [SerializeField] private float m_HitAngle;
    [SerializeField] GameObject lose;
    [SerializeField] NavMeshAgent agent;

    private void Start()
    {

        // TO-DO: Debug messages, should be removed before building
        if (m_HitAngle < 0f)
        {
            Debug.LogWarning("Hit angle cannot be negative.");
            m_HitAngle = Mathf.Abs(m_HitAngle);
        }
        if (m_HitAngle == 0f)
        {
            Debug.LogWarning("Hit angle is not set.");

            TryGetComponent(out Light playerLight);
            if (playerLight != null)
                m_HitAngle = playerLight.spotAngle / 2f;
        }
    }

    void Update()
    {
        Mesh targetMesh = m_PlayerMesh.mesh;
        Vector3 target = m_Player.transform.position;
        target.y -= targetMesh.bounds.size.y / 2;

        Physics.Raycast(transform.position, target - transform.position, out RaycastHit hitInfo);
        if (hitInfo.transform.gameObject == m_Player)
        {
            if (Mathf.Abs(Vector3.Angle(transform.TransformDirection(Vector3.forward), hitInfo.point - transform.position)) < m_HitAngle)
            {
                Debug.Log("Player is being hit!~"); // TO-DO: game over function
                lose.gameObject.SetActive(true);
                agent.isStopped = true;


                if (m_EnableDebug) 
                    Debug.DrawRay(transform.position, target - transform.position, Color.red);
            }
            else if (m_EnableDebug)
            {
                Debug.DrawRay(transform.position, target - transform.position);
            }
        }
       
    }

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log(other);
        if(other.gameObject.CompareTag("Bullet") && gameObject.CompareTag("NoBarrier"))
        {
            Destroy(other.gameObject);
            gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
        Debug.Log("hi");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class BystanderScript : MonoBehaviour
{

    public List<GameObject> Nodes;
    private NavMeshAgent agent;
    private GameObject targetNode;
    private Rigidbody[] ragdollBodies;
    [SerializeField] private LayerMask ragollExcludeLayers;
    public GameObject Player;
    private bool isPossessed = false;
    private bool isStill = false;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }
    void Start()
    {
        Nodes = new List<GameObject>();
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            Nodes.Add(node);
        }
        ChoseTarget();
        Player = PlayerController.Instance.gameObject;
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (!isPossessed)
        {
            if (!isStill)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    ChoseTarget();
                }
            }
        }
        else
        {
            targetNode = Player;
            agent.SetDestination(targetNode.transform.position);
        }
    }

    void ChoseTarget()
    {
        if (Random.Range(0, 5) == 0)
        {
            StartCoroutine(StandStill());
        }
        else
        {
            targetNode = Nodes[Random.Range(0, Nodes.Count)];
            agent.SetDestination(targetNode.transform.position);
        }
    }

    public IEnumerator Possession()
    {
        isPossessed = true;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
        agent.speed = 20f;
        agent.acceleration = 16f;
        yield return new WaitForSeconds(Random.Range(3f, 6f));  
        isPossessed = false;
        agent.speed = 5f;
        agent.acceleration = 8f;
        ChoseTarget();
    }

    public void Frenzy()
    {
        isPossessed = true;
    }
    
    public void OnDeath()
    {
        SetRagdollState(true);
    }

    public IEnumerator StandStill()
    {
        isStill = true;
        yield return new WaitForSeconds(Random.Range(5f, 8f));
        isStill = false;    
    }

    private void SetRagdollState(bool isEnabled)
    {
        foreach (Rigidbody body in ragdollBodies)
        {
            animator.enabled = !isEnabled;
            agent.enabled = !isEnabled;
            body.isKinematic = !isEnabled;

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        SetRagdollState(true);

        Rigidbody hitRigidbody = ragdollBodies.OrderBy(rb => Vector3.Distance(rb.position, hitPoint)).FirstOrDefault();
        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);

        foreach (Rigidbody body in ragdollBodies)
        {
            body.excludeLayers = ragollExcludeLayers;
        }
    }

}

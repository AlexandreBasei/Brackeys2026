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
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (!isPossessed)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                ChoseTarget();
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
        targetNode = Nodes[Random.Range(0, Nodes.Count)];
        agent.SetDestination(targetNode.transform.position);
    }

    public IEnumerator Possession()
    {
        isPossessed = true;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        isPossessed = false;
        ChoseTarget();
    }

    public void OnDeath()
    {
        SetRagdollState(true);
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

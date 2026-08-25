using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BystanderScript : MonoBehaviour
{

    public List<GameObject> Nodes;
    private NavMeshAgent agent;
    private GameObject targetNode;
    public GameObject Player;
    private bool isPossessed = false;
    [SerializeField] private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        Nodes = new List<GameObject>();
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            Nodes.Add(node);
        }
        agent = GetComponent<NavMeshAgent>();
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
        
    }
}

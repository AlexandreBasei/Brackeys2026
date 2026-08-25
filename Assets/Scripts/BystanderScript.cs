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
    private bool isStill = false;
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
        yield return new WaitForSeconds(Random.Range(3f, 6f));  
        isPossessed = false;
        ChoseTarget();
    }

    public void OnDeath()
    {
        
    }

    public IEnumerator StandStill()
    {
        isStill = true;
        yield return new WaitForSeconds(Random.Range(5f, 8f));
        isStill = false;    
    }
}

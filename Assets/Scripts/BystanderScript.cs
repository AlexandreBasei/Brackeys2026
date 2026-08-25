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
    private bool isFaking = false;
    private bool isStill = false;
    [SerializeField] private Animator animator;
    public GameObject neutralMask;
    public GameObject grumpyMask;
    public GameObject happyMask;

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
        Player = PlayerController.Instance.gameObject;
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (!isPossessed && !isFaking)
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
        agent.speed = 13f;
        agent.acceleration = 10f;
        neutralMask.SetActive(false);
        grumpyMask.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        grumpyMask.SetActive(false);
        neutralMask.SetActive(true);
        isPossessed = false;
        agent.speed = 5f;
        agent.acceleration = 8f;
        ChoseTarget();
    }

    public IEnumerator Fakeout()
    {
        isFaking = true;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
        agent.speed = 13f;
        agent.acceleration = 10f;
        neutralMask.SetActive(false);
        happyMask.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        happyMask.SetActive(false);
        neutralMask.SetActive(true);
        isFaking = false;
        agent.speed = 5f;
        agent.acceleration = 8f;
        ChoseTarget();
    }

    public IEnumerator Feral()
    {
        isPossessed = true;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
        agent.speed = 17f;
        agent.acceleration = 16f;
        neutralMask.SetActive(false);
        grumpyMask.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        grumpyMask.SetActive(false);
        neutralMask.SetActive(true);
        isPossessed = false;
        agent.speed = 5f;
        agent.acceleration = 8f;
        ChoseTarget();
    }

    public IEnumerator Smart()
    {
        isPossessed = true;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        isPossessed = false;
        ChoseTarget();
    }

    public void Frenzy()
    {
        isPossessed = true;
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

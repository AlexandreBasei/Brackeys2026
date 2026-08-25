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
    public bool isPossessed = false;
    public bool isFaking = false;
    public bool isStill = false;
    public bool isDead = false;
    public bool isExcluded = false;
    private Animator animator;
    public GameObject neutralMask;
    public GameObject grumpyMask;
    public GameObject happyMask;

    [Header("-------Audio Source-------")]
    [SerializeField] private AudioSource audioSource;

    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip spawnClip;
    [SerializeField] private AudioClip[] impactClips;


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
        if(isDead)
            return;
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
        yield return new WaitForSeconds(Random.Range(6f, 10f));
        isPossessed = false;
        ChoseTarget();
    }

    public void Frenzy()
    {
        isPossessed = true;
        agent.speed = 13f;
        agent.acceleration = 10f;
        targetNode = Player;
        agent.SetDestination(targetNode.transform.position);
    }

    public void OnDeath()
    {
        isDead = true;
        GameManager.Instance.RemoveBystander(this, true);
        PlayImpact();
        this.enabled = false;
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

        OnDeath();
    }

    public void Excluded()
    {
        if (!isExcluded && !isDead)
        {
            isExcluded = true;
            GameManager.Instance.RemoveBystander(this, false);
        }
    }

    public void Included()
    {
        if (!isDead && isExcluded)
        {
            isExcluded = false;
            GameManager.Instance.AddBystander(this);
        }
    }

    public void PlaySpawnSound()
    {
        audioSource.PlayOneShot(spawnClip);
    }

    public void PlayImpact()
    {
        PlaySFX(impactClips[Random.Range(0, impactClips.Length)]);
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Alexis : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Route Points")]
    public Transform officeDoorPoint;
    public Transform exitBuildingPoint;
    public Transform keyBuildingPoint;

    [Header("Vision Settings")]
    public float viewRadius = 6f;
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Dialogue")]
    [SerializeField] private NPCDialogue npcDialogue;

    [Header("Key Logic")]
    public KeySlot keySlot;
    public Transform player;
    public float waitAtKeySpotTime = 2f;

    private float waitTimer;
    private SkinnedMeshRenderer alexisRenderer;
    private bool playerCaught = false;

    public float gameOverDelay = 3f;

    private enum AlexisState
    {
        Hidden,
        GoingToOfficeDoor,
        ExitingBuilding,
        GoingToKeySpot,
        CheckingKeySpot,
        ReturningToOffice,
        TeleportToPlayer
    }

    private AlexisState currentState = AlexisState.Hidden;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        alexisRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        agent.enabled = false;
        alexisRenderer.enabled = false;
    }

    void Update()
    {
        if (!agent.enabled || playerCaught) return;

        animator.SetFloat("Speed", agent.velocity.magnitude);
        CheckVision();

        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
        {
            OnDestinationReached();
        }

        if (currentState == AlexisState.CheckingKeySpot)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtKeySpotTime)
            {
                CheckKeyResult();
            }
        }
    }

    public void AppearAndStartRoute()
    {
        alexisRenderer.enabled = true;
        agent.enabled = true;

        currentState = AlexisState.GoingToOfficeDoor;
        agent.SetDestination(officeDoorPoint.position);
    }

    void OnDestinationReached()
    {
        switch (currentState)
        {
            case AlexisState.GoingToOfficeDoor:
                currentState = AlexisState.ExitingBuilding;
                agent.SetDestination(exitBuildingPoint.position);
                break;

            case AlexisState.ExitingBuilding:
                currentState = AlexisState.GoingToKeySpot;
                agent.SetDestination(keyBuildingPoint.position);
                break;

            case AlexisState.GoingToKeySpot:
                currentState = AlexisState.CheckingKeySpot;
                agent.isStopped = true;
                waitTimer = 0f;
                break;

            case AlexisState.ReturningToOffice:
                agent.isStopped = true;
                animator.SetTrigger("OpenDoor");
                break;
        }
    }

    void CheckKeyResult()
    {
        agent.isStopped = false;

        if (keySlot.hasKey)
        {
            currentState = AlexisState.ReturningToOffice;
            agent.SetDestination(officeDoorPoint.position);
            Debug.Log("Alexis: Key found, returning to office!");
        }
        else
        {
            TeleportToPlayer();
        }
    }

    void TeleportToPlayer()
    {
        currentState = AlexisState.TeleportToPlayer;
        playerCaught = true;

        agent.isStopped = true;
        agent.ResetPath();

        agent.Warp(player.position + player.forward * 1.5f);

        transform.LookAt(player);

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Alert");

        npcDialogue.StartConversationByIndex(2);

        Debug.Log("Alexis atrapó al jugador");

        StartCoroutine(LoadNextSceneAfterDelay(gameOverDelay));
    }

    void CheckVision()
    {
        Collider[] playersInRange =
            Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (playersInRange.Length == 0) return;

        Transform p = playersInRange[0].transform;
        Vector3 dir = (p.position - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, dir);

        if (angle <= viewAngle / 2f)
        {
            float dist = Vector3.Distance(transform.position, p.position);

            if (!Physics.Raycast(transform.position + Vector3.up * 1.6f,
                                 dir,
                                 dist,
                                 obstacleMask))
            {
                OnPlayerSeen();
            }
        }
    }

    void OnPlayerSeen()
    {
        if (playerCaught) return;

        playerCaught = true;
        agent.isStopped = true;

        animator.SetTrigger("Alert");
        npcDialogue.StartConversationByIndex(1);
        StartCoroutine(LoadNextSceneAfterDelay(gameOverDelay));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    private System.Collections.IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
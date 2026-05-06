using UnityEngine;
using UnityEngine.AI;

public class GuardPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;

    [Header("Look Around")]
    public float lookAroundAngle = 45f;
    public float lookAroundSpeed = 2f;

    [Header("Player Detection")]
    public Transform player;
    public Transform playerCamera;

    public float proximityDetectionRange = 7f;
    public float visionDetectionRange = 12f;
    public float fieldOfView = 110f;

    [Header("Chase")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;
    public float losePlayerDistance = 16f;

    [Header("Catch")]
    public float catchDistance = 2.5f;

    [Header("Flashlight")]
    public Light flashlight;
    public AudioSource alertAudio;

    [Header("Final Chase Rally")]
    public float rallySpeed = 4f;
    public float rallyArrivalDistance = 1.5f;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;
    private bool isAlerted = false;

    private bool isLookingAround = false;
    private float baseYRotation;
    private int lookDirection = 1;

    private bool finalChaseMode = false;

    private bool rallyBeforeChaseMode = false;
    private Transform rallyTarget;
    private float storedFinalChaseSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (flashlight != null)
            flashlight.enabled = true;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    void Update()
    {
        if (player == null) return;
        if (agent == null) return;

        if (rallyBeforeChaseMode)
        {
            MoveToRallyPointBeforeChase();
            return;
        }

        if (!finalChaseMode)
        {
            if (patrolPoints.Length == 0) return;
            CheckPlayerDetection();
        }

        if (isAlerted)
            ChaseMode();
        else
            PatrolMode();
    }

    void CheckPlayerDetection()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= proximityDetectionRange)
        {
            isAlerted = true;
            return;
        }

        if (distanceToPlayer > visionDetectionRange)
        {
            if (distanceToPlayer > losePlayerDistance)
                isAlerted = false;

            return;
        }

        Vector3 flatDirection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z);
        float angle = Vector3.Angle(transform.forward, flatDirection);

        if (angle <= fieldOfView * 0.5f)
        {
            isAlerted = true;
        }
        else
        {
            if (distanceToPlayer > proximityDetectionRange)
                isAlerted = false;
        }
    }

    void PatrolMode()
    {
        agent.speed = patrolSpeed;

        if (flashlight != null)
            flashlight.enabled = true;

        if (alertAudio != null && alertAudio.isPlaying)
            alertAudio.Stop();

        if (isLookingAround)
        {
            LookAround();
            return;
        }

        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                isLookingAround = true;
                waitTimer = waitTimeAtPoint;

                agent.isStopped = true;
                baseYRotation = transform.eulerAngles.y;
                lookDirection = 1;
            }
        }

        if (isWaiting && !isLookingAround)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[currentPointIndex].position);
                isWaiting = false;
            }
        }
    }

    void LookAround()
    {
        float targetAngle = baseYRotation + (lookAroundAngle * lookDirection);
        float currentY = transform.eulerAngles.y;

        float newY = Mathf.MoveTowardsAngle(currentY, targetAngle, lookAroundSpeed * 50f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);

        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetAngle)) < 1f)
        {
            if (lookDirection == 1)
            {
                lookDirection = -1;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, baseYRotation, 0f);
                isLookingAround = false;
            }
        }
    }

    void ChaseMode()
    {
        isWaiting = false;
        isLookingAround = false;

        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        Vector3 target = player.position;
        target.y = transform.position.y;
        transform.LookAt(target);

        if (flashlight != null)
            flashlight.enabled = !finalChaseMode;

        if (alertAudio != null && !alertAudio.isPlaying)
            alertAudio.Play();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= catchDistance)
        {
            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.LoseByGuard();
            }
            return;
        }

        if (!finalChaseMode && distanceToPlayer > losePlayerDistance)
        {
            isAlerted = false;
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    public void MoveToRallyPointThenChase(Transform rallyPoint, float newChaseSpeed)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent == null)
        {
            return;
        }

        rallyTarget = rallyPoint;
        storedFinalChaseSpeed = newChaseSpeed;

        finalChaseMode = false;
        rallyBeforeChaseMode = true;
        isAlerted = false;
        isWaiting = false;
        isLookingAround = false;

        agent.isStopped = false;
        agent.speed = rallySpeed;

        if (flashlight != null)
        {
            flashlight.enabled = false;
        }

        if (alertAudio != null && alertAudio.isPlaying)
        {
            alertAudio.Stop();
        }

        if (rallyTarget != null)
        {
            agent.SetDestination(rallyTarget.position);
        }
        else
        {
            ForceFinalChase(newChaseSpeed);
        }
    }

    private void MoveToRallyPointBeforeChase()
    {
        if (rallyTarget == null)
        {
            rallyBeforeChaseMode = false;
            ForceFinalChase(storedFinalChaseSpeed);
            return;
        }

        agent.isStopped = false;
        agent.speed = rallySpeed;
        agent.SetDestination(rallyTarget.position);

        if (!agent.pathPending && agent.remainingDistance <= rallyArrivalDistance)
        {
            rallyBeforeChaseMode = false;
            ForceFinalChase(storedFinalChaseSpeed);
        }
    }

    public void ForceFinalChase(float newChaseSpeed)
    {
        rallyBeforeChaseMode = false;
        finalChaseMode = true;
        isAlerted = true;
        isWaiting = false;
        isLookingAround = false;

        chaseSpeed = newChaseSpeed;
        losePlayerDistance = 999f;
        proximityDetectionRange = 999f;
        visionDetectionRange = 999f;

        if (flashlight != null)
        {
            flashlight.enabled = false;
        }

        if (agent != null && player != null)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityDetectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionDetectionRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left * visionDetectionRange);
        Gizmos.DrawRay(transform.position, right * visionDetectionRange);

        Gizmos.color = Color.magenta;
        if (rallyTarget != null)
        {
            Gizmos.DrawSphere(rallyTarget.position, 0.4f);
            Gizmos.DrawLine(transform.position, rallyTarget.position);
        }
    }
}
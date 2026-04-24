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

    // Si el jugador entra a este rango, lo detecta aunque esté atrás.
    public float proximityDetectionRange = 7f;

    // Distancia máxima para detección frontal.
    public float visionDetectionRange = 12f;

    // Ángulo del cono de visión.
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

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;
    private bool isAlerted = false;

    // Variables para la animación de vigilancia
    private bool isLookingAround = false;
    private float baseYRotation;
    private int lookDirection = 1;

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
        if (patrolPoints.Length == 0) return;

        CheckPlayerDetection();

        if (isAlerted)
            ChaseMode();
        else
            PatrolMode();
    }

    // Revisa si el jugador entra en el rango de detección.
    void CheckPlayerDetection()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Detección cercana: aunque esté detrás.
        if (distanceToPlayer <= proximityDetectionRange)
        {
            isAlerted = true;
            return;
        }

        // Si está demasiado lejos, no lo detecta.
        if (distanceToPlayer > visionDetectionRange)
        {
            if (distanceToPlayer > losePlayerDistance)
                isAlerted = false;

            return;
        }

        // Detección frontal por cono de visión.
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

    // Patrullaje normal entre puntos.
    void PatrolMode()
    {
        agent.speed = patrolSpeed;

        if (flashlight != null)
            flashlight.enabled = true;

        if (alertAudio != null && alertAudio.isPlaying)
            alertAudio.Stop();

        // Si está en momento de vigilancia, no avanza:
        // solo rota a los lados para revisar.
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

    // Hace que el guardia mire a izquierda y derecha
    // antes de continuar patrullando.
    void LookAround()
    {
        float targetAngle = baseYRotation + (lookAroundAngle * lookDirection);
        float currentY = transform.eulerAngles.y;

        float newY = Mathf.MoveTowardsAngle(currentY, targetAngle, lookAroundSpeed * 50f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);

        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetAngle)) < 1f)
        {
            // Si ya miró a un lado, cambia al otro.
            if (lookDirection == 1)
            {
                lookDirection = -1;
            }
            else
            {
                // Cuando ya miró ambos lados, vuelve al centro y termina vigilancia.
                transform.rotation = Quaternion.Euler(0f, baseYRotation, 0f);
                isLookingAround = false;
            }
        }
    }

    // Persecución del jugador.
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
            flashlight.enabled = true;

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

        if (distanceToPlayer > losePlayerDistance)
        {
            isAlerted = false;
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    // Dibuja rangos de detección en la escena.
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
    }
}
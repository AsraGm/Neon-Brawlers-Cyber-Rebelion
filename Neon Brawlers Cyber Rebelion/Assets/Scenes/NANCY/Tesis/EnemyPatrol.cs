using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private NavMeshAgent agent;
    private FieldOfView fieldOfView;

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex;
    private Vector3 target;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseStoppingDistance = 1.5f;
    [SerializeField] private float patrolStoppingDistance = 0.5f;
    [SerializeField] private float chaseRadius = 15f;

    [Header("Movement Settings")]
    [SerializeField] private float angularSpeed = 360f;
    [SerializeField] private float acceleration = 10f;

    public bool isStunned /*{ get; private set; }*/ = false;
    public bool isChasing /*{ get; private set; }*/ = false;
    private bool isWaiting = false;
    private Transform player;
    private Coroutine idleCoroutine;
    private RobotAttack robotAttack;
    public bool alertedByDrone = false;
    public bool canAttack = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();

        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.stoppingDistance = patrolStoppingDistance;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
            agent.autoBraking = true;
            UpdateDestination();
        }
        player = GameObject.Find("Player").transform;
        robotAttack = GetComponent<RobotAttack>();
    }

    void Update()
    {
        if (isStunned) return;

        CheckChaseRange();

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else if (!isWaiting)
        {
            Patrol();
        }

        HandleRotation();
    } 

    void CheckChaseRange()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= chaseRadius;

        if (alertedByDrone || (playerInRange && fieldOfView != null && fieldOfView.canSeePlayer))
        {
            if (!isChasing)
            {
                StartChasing();
            }
        }

        if (isChasing)
        {
            if (!alertedByDrone && !playerInRange)
            {
                StopChasing();
            }
            else if (alertedByDrone && distanceToPlayer > chaseRadius * 1.5f)
            {
                alertedByDrone = false;
                StopChasing();
            }
        }

    }

    public void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = chaseStoppingDistance;

        // Detiene la coroutine de espera si está activa
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
            isWaiting = false;
        }
    }

    void StopChasing()
    {
        isChasing = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;
        UpdateDestination();
    }

    void ChasePlayer()
    {
        if (Vector3.Distance(agent.destination, player.position) > 1f)
        {
            agent.SetDestination(player.position);
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (idleCoroutine == null)
            {
                idleCoroutine = StartCoroutine(WaitAtWaypoint());
            }
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

        IterateWaypointIndex();
        UpdateDestination();

        isWaiting = false;
        idleCoroutine = null;
    }

    void HandleRotation()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * agent.angularSpeed / 120f
            );
        }
    }

    void UpdateDestination()
    {
        if (agent != null && waypoints.Length > 0)
        {
            target = waypoints[waypointIndex].position;
            agent.SetDestination(target);
        }
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    public void ApplyStun(float duracion)
    {
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine(duracion));
        }
    }

    private IEnumerator StunCoroutine(float duracion)
    {
        isStunned = true;

        if (robotAttack != null)
        {
            robotAttack.StopRobotAttack();
        }

        // parar animaciones

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
            isWaiting = false;
        }

        yield return new WaitForSeconds(duracion);

        isStunned = false;
        canAttack = true;

        if (robotAttack != null)
        {
            robotAttack.RobotCanAttack();
        }

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    public void StopStun()
    {
        if (isStunned)
        {
            StopAllCoroutines();
            isStunned = false;
            agent.isStopped = false;
            StartChasing();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}

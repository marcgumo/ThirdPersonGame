using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum MovementStates
    {
        Initial, Patrolling, Chase, Attack
    }

    private MovementStates enemyState;

    [Header("Patrolling settings")]
    [SerializeField] private float patrollingStopTime = 2f;
    [SerializeField] private List<Transform> patroList;

    Vector3 walkPoint;
    bool walkPointSet;
    float currentPatrollingStopTime;
    int currentPatrollingPosition = 0;

    NavMeshAgent agent;

    [Header("Chase settings")]
    [SerializeField] private float sightRange = 7f;
    [SerializeField] private LayerMask playerLayer;
    bool playerInSightRange;

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField, Range(0.0f, 360.0f)] private float sightAngle = 130;

    Transform player;

    [Header("Attack settings")]
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private float timeBetweenAttacks = 0.75f;
    [SerializeField] private float rotateSpeed = 4;
    bool alreadyAttack;
    bool playerInAttackRange;

    [Header("Animation Settings")]
    [SerializeField] private float acceleration = 1.5f;
    [SerializeField] private float deceleration = 0.75f;
    [SerializeField] private SphereCollider attackCollider;
    float enemySpeed;
    Animator anim;

    int attackCounter;


    void Start()
    {
        enemyState = MovementStates.Initial;

        agent = GetComponent<NavMeshAgent>();
        currentPatrollingStopTime = patrollingStopTime;
        walkPointSet = true;
        walkPoint = patroList[currentPatrollingPosition].position;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        StateUpdate();

        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        StartCoroutine(SightRoutine());
    }

    private void ChangeState(MovementStates newState)
    {
        //Outcoming triggering condition
        switch (enemyState)
        {
            case MovementStates.Initial:
                break;
            case MovementStates.Patrolling:
                break;
            case MovementStates.Chase:
                break;
            case MovementStates.Attack:
                break;
            default:
                break;
        }

        //Incoming triggering condition
        switch (newState)
        {
            case MovementStates.Initial:
                break;
            case MovementStates.Patrolling:
                agent.speed = 1.75f;
                agent.angularSpeed = 150;
                break;
            case MovementStates.Chase:
                agent.speed = 3.5f;
                agent.angularSpeed = 250;

                attackCounter = 0;
                anim.SetInteger("attack", attackCounter);
                break;
            case MovementStates.Attack:
                break;
            default:
                break;
        }

        enemyState = newState;
    }

    private void StateUpdate()
    {
        switch (enemyState)
        {
            case MovementStates.Initial:
                ChangeState(MovementStates.Patrolling);
                break;
            case MovementStates.Patrolling:
                Patrolling();
                if (playerInSightRange && !PathInvalid())
                    ChangeState(MovementStates.Chase);
                break;
            case MovementStates.Chase:
                ChasePlayer();

                if (!playerInSightRange)
                    ChangeState(MovementStates.Patrolling);

                if (playerInAttackRange)
                    ChangeState(MovementStates.Attack);
                break;
            case MovementStates.Attack:
                if (!playerInAttackRange && !alreadyAttack)
                    ChangeState(MovementStates.Chase);
                AttackPlayer();
                break;
            default:
                break;
        }
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchPatrolPoint();
        else agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 0.1f)
        {
            if (currentPatrollingStopTime <= 0)
                walkPointSet = false;
            else
                currentPatrollingStopTime -= Time.deltaTime;

            if (enemySpeed > 0)
                enemySpeed -= deceleration * Time.deltaTime;
            else
                enemySpeed = 0;
        }
        else
        {
            if (enemySpeed > 0.4f)
                enemySpeed -= deceleration * Time.deltaTime;
            else
            {
                if (enemySpeed < 0.4f)
                    enemySpeed += acceleration * Time.deltaTime;
                else
                    enemySpeed = 0.4f;
            }
        }

        anim.SetFloat("speed", enemySpeed);
    }

    private void SearchPatrolPoint()
    {
        currentPatrollingStopTime = patrollingStopTime;
        currentPatrollingPosition++;

        if (currentPatrollingPosition >= patroList.Count)
            currentPatrollingPosition = 0;

        walkPoint = patroList[currentPatrollingPosition].position;
        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        if (PathInvalid())
            ChangeState(MovementStates.Patrolling);

        if (enemySpeed < 0.85f)
            enemySpeed += acceleration * Time.deltaTime;
        else
            enemySpeed = 0.85f;

        anim.SetFloat("speed", enemySpeed);
    }

    private bool PathInvalid()
    {
        NavMeshPath path = agent.path;
        agent.CalculatePath(player.position, path);

        if (path.status == NavMeshPathStatus.PathInvalid)
            return true;

        return false;
    }

    private void AttackPlayer()
    {
        if (enemyState == MovementStates.Chase)
            return;

        agent.SetDestination(transform.position);

        Vector3 targetposition = new Vector3(player.position.x, transform.position.y, player.position.z);

        //transform.LookAt(targetposition);
        Quaternion lookRotation = Quaternion.LookRotation(targetposition - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        if (!alreadyAttack)
        {
            //Debug.Log("Attacking player");
            attackCounter++;
            if (attackCounter > 3)
                attackCounter = 0;

            anim.SetInteger("attack", attackCounter);

            alreadyAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        if (enemySpeed > 0)
            enemySpeed -= deceleration * 2 * Time.deltaTime;
        else
            enemySpeed = 0;

        anim.SetFloat("speed", enemySpeed);
    }

    private void ResetAttack()
    {
        alreadyAttack = false;
    }

    public void SetAttackCollider(bool value)
    {
        if (enemyState == MovementStates.Chase)
            return;

        attackCollider.enabled = value;
    }

    private IEnumerator SightRoutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + 0.2f)
        {
            SightCheck();
            yield return null;
        }
    }

    private void SightCheck()
    {
        Collider[] sightChecks = Physics.OverlapSphere(transform.position, sightRange, playerLayer);

        if (sightChecks.Length != 0)
        {
            Transform target = sightChecks[0].transform;

            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < sightAngle * 0.5)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
                    playerInSightRange = true;
                else
                    playerInSightRange = false;
            }
            else
                playerInSightRange = false;
        }
        else if (playerInSightRange)
            playerInSightRange = false;

    }

    public float GetSightAngle()
    {
        return sightAngle;
    }

    public float GetSightRange()
    {
        return sightRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
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

    Transform player;

    void Start()
    {
        enemyState = MovementStates.Initial;

        agent = GetComponent<NavMeshAgent>();
        currentPatrollingStopTime = patrollingStopTime;
        walkPointSet = true;
        walkPoint = patroList[currentPatrollingPosition].position;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        StateUpdate();

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
    }

    private void ChangeState(MovementStates newState)
    {
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

        switch (newState)
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
                break;
            case MovementStates.Chase:
                break;
            case MovementStates.Attack:
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
            else currentPatrollingStopTime -= Time.deltaTime;
        }
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



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
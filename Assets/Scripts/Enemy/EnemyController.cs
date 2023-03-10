using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MovementStates
    {
        Initial, Patrolling, Chase, Attack
    }

    private MovementStates enemyState;

    void Start()
    {
        enemyState = MovementStates.Initial;
    }

    void Update()
    {
        StateUpdate();
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
                break;
            case MovementStates.Chase:
                break;
            case MovementStates.Attack:
                break;
            default:
                break;
        }
    }
}

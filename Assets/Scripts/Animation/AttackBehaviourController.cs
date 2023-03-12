using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviourController : StateMachineBehaviour
{
    [Header("Attack setings")]
    [SerializeField] private float startAttack = 0.35f;
    [SerializeField] private float endAttack = 0.7f;
    [SerializeField] private bool isPlayer;

    PlayerController player;

    EnemyController enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isPlayer)
            player = animator.GetComponentInParent<PlayerController>();
        else
            enemy = animator.GetComponentInParent<EnemyController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool enableAttack = stateInfo.normalizedTime > startAttack && stateInfo.normalizedTime < endAttack;

        if (isPlayer)
            player.SetAttackCollider(enableAttack);
        else
            enemy.SetAttackCollider(enableAttack);
    }
}

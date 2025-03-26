using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Ragdoll ragdoll;
    private bool interactionDisable;
    
    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
        ragdoll = enemy.GetComponent<Ragdoll>();
    }

    public override void Enter()
    {   
        base.Enter();
        enemy.ragdoll.RagdollActive(true);
        interactionDisable = false;

        enemy.animator.enabled = false;
        enemy.agent.isStopped = true;
        stateTimer = 3;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        
        base.Update();

        DisableInteraction();
    }

    private void DisableInteraction()
    {
        if (stateTimer < 0 && interactionDisable == false)
        {
            interactionDisable = true;
            enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.ColliderActive(false);
        }
    }
}

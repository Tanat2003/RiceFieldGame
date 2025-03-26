using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Boss : EnemyState
{
    private EnemyBoss enemy;
    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyBoss;
    }

    public override void Enter()
    {
        base.Enter();
        
        stateTimer = enemyBase.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(enemy.inBattleMode &&enemy.PlayerInAttackRange())
        {
            enemy.FaceTarget(enemy.player.transform.position, 1000);
            stateMachine.ChangeState(enemy.attackState);
        }
        if(stateTimer <0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}

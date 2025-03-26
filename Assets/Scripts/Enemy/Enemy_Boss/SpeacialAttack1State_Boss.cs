using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeacialAttack1State_Boss : EnemyState
{
    //JumpAttack
    private EnemyBoss enemy;
    private Vector3 lastPlayerPos;

    private float jumpAttackMovementSpeed;
    public SpeacialAttack1State_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyBoss;
    }

    public override void Enter()
    {
        base.Enter();
        lastPlayerPos = enemy.player.position;
        float distanceToPlayer= Vector3.Distance(lastPlayerPos,enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer/enemy.travelTimeToTarget; //ไม่ว่าไกลแค่ไหนจะกระโดดถึงในระยะเวลาเท่าเดิม
        enemy.FaceTarget(lastPlayerPos, 500);

        enemy.bossVisual.PlaceLandingZone(lastPlayerPos);
        enemy.bossVisual.EnableWeaponTrail(true);
        if(enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.agent.isStopped = false;
            enemy.agent.speed = enemy.moveSpeed;
            enemy.agent.SetDestination(lastPlayerPos);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetJumpAttackCooldown();
        enemy.bossVisual.EnableWeaponTrail(false);
    }

    public override void Update()
    {
        base.Update();
        Vector3 enemyPos =enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive();
        if(enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemyPos,lastPlayerPos, jumpAttackMovementSpeed*Time.deltaTime);
        }
        if(triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}

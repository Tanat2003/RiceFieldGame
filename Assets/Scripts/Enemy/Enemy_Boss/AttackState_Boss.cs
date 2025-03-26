using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private EnemyBoss enemy;
    public float lastTimeAttack {  get; private set; }  
    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyBoss;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.FaceTarget(enemy.player.position, 15);
        enemy.bossVisual.EnableWeaponTrail(true);
        enemy.animator.SetFloat("AttackIndex", Random.Range(0, 2));
        enemy.agent.isStopped = true;
        stateTimer = 1f;
        
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAttack = Time.time;
        enemy.bossVisual.EnableWeaponTrail(false);
    }

    public override void Update()
    {
        base.Update();
        
        if (triggerCalled)
        {
            
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }
}

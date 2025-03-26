using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastTimeUpdateDestination;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.chaseSpeed;
        enemy.agent.isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        enemyBase.FaceTarget(GetNextPathPoint());
        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }

    private bool CanUpdateDestination() //����Ҷ֧�����ѾഷDestination���ѧ�͹������Ҥ�Ŵ�ǹ���0.25
                                        //��Ҷ֧��������¹�����������¹�ش����ai�繵��˹觢ͧplayer � �͹���
    {
        if (Time.time > lastTimeUpdateDestination + .25F)
        {
            lastTimeUpdateDestination = Time.time;
            return true;
        }
        return false;
    }


}

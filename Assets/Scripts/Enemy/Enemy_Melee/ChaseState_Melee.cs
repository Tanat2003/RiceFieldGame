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

    private bool CanUpdateDestination() //àªç¤ÇèÒ¶Ö§àÇÅÒÍÑ¾à´·DestinationÃÖÂÑ§µÍ¹¹ÕéàÇÅÒ¤ÙÅ´ÒÇ¹ìà»ç¹0.25
                                        //¶éÒ¶Ö§àÇÅÒà»ÅÕèÂ¹áÅéÇãËéà»ÅÕèÂ¹¨Ø´ËÁÒÂaià»ç¹µÓáË¹è§¢Í§player ³ µÍ¹¹Ñé¹
    {
        if (Time.time > lastTimeUpdateDestination + .25F)
        {
            lastTimeUpdateDestination = Time.time;
            return true;
        }
        return false;
    }


}

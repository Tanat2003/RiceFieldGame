using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private EnemyBoss enemy;
    private Vector3 destination;
    private float actionTimer; //CooldownÀ≈—ß„™È °‘≈À√◊ÕSpeacialattack®–‰¡Ë„ÀÈ “¡“√∂∑”2Õ¬Ë“ßπ’ÈµËÕ‡π◊ËÕß°—π‰¥È
    private float timeBeforeSpeedUp = 5f;
    private bool speedUpActive;
    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyBoss;

    }

    public override void Enter()
    {
        base.Enter();
        SpeedReset();
        enemy.agent.isStopped = false;
        destination = enemy.GetPartrolDestination();
        enemy.agent.SetDestination(destination);
        actionTimer = enemy.actionCoolDown;
    }

    private void SpeedReset()
    {
        speedUpActive = false;
        enemy.animator.SetFloat("MoveIndex", 0);
        enemy.animator.SetFloat("MoveAnimSpeedMutiplier", 1);
        enemy.agent.speed = enemy.moveSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.inBattleMode)
        {
            if(ShouldSpeedUp())
            {

                enemy.agent.speed = enemy.chaseSpeed;
                enemy.animator.SetFloat("MoveIndex", 1);
                enemy.animator.SetFloat("MoveAnimSpeedMutiplier", 1.5f);
                speedUpActive = true;

            }

            Vector3 playerPosition = enemy.player.position;
            enemy.agent.SetDestination(playerPosition);

            if(actionTimer <0) // ÿË¡„™ÈSpeacilAttackÀ√◊ÕAbility‡¡◊ËÕActiontimer <0
            {
               stateMachine.ChangeState(enemy.abilityState);
               // PerformRandomAction();   
            }
            
            else if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);

            }



        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < .25f)
            {
                stateMachine.ChangeState(enemy.idleState);
            }

        }


    }
    
private void PerformRandomAction()
    {
        actionTimer = enemy.actionCoolDown;
        if (Random.Range(0, 2)== 1)
        {
            if (enemy.CanDoAbility())
            {
                stateMachine.ChangeState(enemy.abilityState);
            }

        }
        else //if (Random.Range(0, 2) == 1)
        {
            if (enemy.CanDoJumpAttack())
            {
                stateMachine.ChangeState(enemy.speacialAttack1State);
            }


    }
    }
    private bool ShouldSpeedUp()
    {
        if (speedUpActive)
            return false;
        if(Time.time > enemy.attackState.lastTimeAttack + timeBeforeSpeedUp)
        {
            return true;
        }
        return false;
    }
}

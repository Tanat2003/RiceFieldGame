using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private EnemyBoss enemy;
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyBoss;
    }

    public override void Enter()
    {

        base.Enter();
        enemy.agent.isStopped = true;
        stateTimer = enemy.flameThrowDuration;
        enemy.bossVisual.EnableWeaponTrail(true);
    }


    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.position);
        if (stateTimer < 0 && enemy.bossWeaponType == BossWeaponType.FireThrow)
        {
            DisableFlameThrow();
        }
        if (triggerCalled)
        {
            
            stateMachine.ChangeState(enemy.moveState);
        }
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        if (enemy.bossWeaponType == BossWeaponType.FireThrow)
        {
            enemy.ActivateFlameThrow(true);
            enemy.bossVisual.DischargeBattery();

        }
        if(enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
        }
    }
    public void DisableFlameThrow() //ãªéËÂØ´Ê¡ÔÅ¾è¹ä¿µÍ¹à¢éÒÊÙèÊàµ¨µÒÂ
    {
        if (enemy.bossWeaponType != BossWeaponType.FireThrow)
        {
            return;
        }
        enemy.ActivateFlameThrow(false);
    }

    public override void Exit()
    {
        base.Exit();
        
        enemy.SetAbilityCooldown();
        enemy.bossVisual.ResetBatteries();
        enemy.bossVisual.EnableWeaponTrail(false);
    }
}

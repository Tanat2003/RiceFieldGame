using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationEvent : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Melee enemyMelee;
    private EnemyBoss enemyBoss;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
        enemyBoss = GetComponentInParent<EnemyBoss>();
    }
    public void AnimationTrigger()=>enemy.AnimationTrigger();
    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMove() => enemy.ActivateManualMovement(false); 
    public void StartManualRotation()=> enemy.ActivateManualMovement(true);
    public void StopManualRotation ()=> enemy.ActivateManualMovement(false);

    public void AbilityEvent() => enemy.AbilityTrigger(); //‡√’¬°‡¡∏Õ¥®“°ª√–‡¿∑enemy∑’Ë‡√“override
    public void BossJumpImpact()
    {
        enemyBoss?.JumpImpact();
    }
    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(true);
    }
    public void FinishedMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(false);
    }
}

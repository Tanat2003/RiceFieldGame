using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50f;
    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.UpdateAttackData();   
        enemy.EnableWeaponModel(true);
        enemy.visuals.EnableWeaponTrail(true);


        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.animator.SetFloat("AttackAnimationSpeed",enemy.attackData.animationSpeed);
        enemy.animator.SetFloat("AttackIndex",enemy.attackData.attackIndex);
        enemy.animator.SetFloat("SlashAttackIndex", Random.Range(0,5));
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        attackDirection = enemy.transform.position+ (enemy.transform.forward*MAX_ATTACK_DISTANCE);  
        //ตอนแรกศัตรูโจมตีเราไม่โดนเลยถ้าเราวิ่งเพราะจะเอาตำแหน่งล่าสุดที่เรายืนไป 
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack();

        enemy.visuals.EnableWeaponTrail(false);

    }

    private void SetupNextAttack()
    {
        int recoverIndex = PlayerClose() ? 1 : 0;
        enemy.animator.SetFloat("RecoveryIndex", recoverIndex);
        enemy.attackData = UpdateAttackData();
    }

    public override void Update()
    {
        base.Update();
        if(enemy.ManualRotationActive())
        {
             enemy.FaceTarget(enemy.player.position);
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);

        }

        
        if(enemy.ManualMovementActive())
        {
            enemy.transform.position //fixตำแหน่งที่ศัตรูจะโจมตีเรา
            = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
        }
          
        if(triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.recoveryState);
            }
            else
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
    }
    private bool PlayerClose()=>Vector3.Distance(enemy.transform.position,enemy.player.position)<=1;
    private MeleeAttackData UpdateAttackData()
    {
        List<MeleeAttackData> validAttack = new List<MeleeAttackData>(enemy.attackList);

        if(PlayerClose())
        {//ถ้าผู้เล่นอยู่ใกล้ให้ลบparameterที่สามารถรีเทรินattacktype == AttackType_Melee.Chargeออกจากlist
            //ตอนผู้เล่นอยู่ใกล้ไม่สามารถใช้อนิเมชั่นแบบพุ่งเข้าชาร์จได้
            validAttack.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);
        }
        int randomattak =Random.Range(0,validAttack.Count);
        return validAttack[randomattak]; //รีเทรินรูปแบบโจมตีแบบสุ่มไป


    }
}

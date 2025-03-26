using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 movementDirection;
    private const float MAX_MOVEMENT_DISTANCE = 20;
    private float moveSpeed;


    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.EnableWeaponModel(true); //À¬‘∫Õ“«ÿ∏¢÷Èπ¡“¢«È“ß
        moveSpeed = enemy.moveSpeed;
        movementDirection =
             enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }
    public override void Exit()
    {
        base.Exit();
        enemy.moveSpeed = moveSpeed;
        enemy.animator.SetFloat("RecoveryIndex", 0);
    }
    public override void Update()
    {
        base.Update();
        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);

        }
        if (enemy.ManualMovementActive())
        {
            enemy.transform.position //fixµ”·ÀπËß∑’Ë»—µ√Ÿ®–‚®¡µ’‡√“
            = Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.moveSpeed * Time.deltaTime);
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.recoveryState);
        }
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject newitemThrow = Object_Pool.instance.GetObject(enemy.throwPrefab,enemy.throwStartPoint);
        newitemThrow.transform.position = enemy.throwStartPoint.position;
        newitemThrow.GetComponent<EnemyThrow>().EnemyThrowSetup(enemy.throwFlySpeed, enemy.player, enemy.throwTimer,enemy.throwDamage);


    }
}

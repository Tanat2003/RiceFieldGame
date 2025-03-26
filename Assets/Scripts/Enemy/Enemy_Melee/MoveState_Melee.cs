using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter() //µÕπ∑’Ëmelee‡¢È“‡ µ®
    {
        base.Enter();
        enemy.agent.speed = enemy.moveSpeed;
        destination = enemy.GetPartrolDestination(); //assignµ”·ÀπËßdestination®“°Õ“‡√¬Ïpartrol
        enemy.agent.SetDestination(destination); //°”Àπ¥µ”·ÀπËß„ÀÈai‡¥‘π
    }

    public override void Exit()
    {
        base.Exit();


    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());
        if (enemy.agent.remainingDistance <= enemyBase.agent.stoppingDistance + 0.05f)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

}

using System.Diagnostics;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }
    public void Initialize(EnemyState State)
    {
        currentState = State;
        
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }


}

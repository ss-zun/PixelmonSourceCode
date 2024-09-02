public class EnemyIdleState : IdleState
{
    EnemyFSM enemyStateMachine;

    public EnemyIdleState(EnemyFSM stateMachine) : base(stateMachine)
    {
        enemyStateMachine = stateMachine;
    }

}
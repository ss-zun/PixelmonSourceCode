using UnityEngine;

public class EnemyDieState : DieState
{
    private new EnemyFSM fsm;

    public EnemyDieState(EnemyFSM fsm) 
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        fsm.enemy.enemyCollider.enabled = false;
        fsm.rb.bodyType = RigidbodyType2D.Kinematic;
        GameManager.Instance.NotifyEnemyDie(fsm.enemy);
        base.Enter();
    }

    public override void Exit()
    {
        fsm.rb.bodyType = RigidbodyType2D.Dynamic;
        base.Exit();
    }
}
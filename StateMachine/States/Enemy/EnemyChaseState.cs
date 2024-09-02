
using UnityEngine;

public class EnemyChaseState : MoveState
{
    private new EnemyFSM fsm;

    public EnemyChaseState(EnemyFSM fsm) 
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Execute()
    {
        base.Execute();
        MoveTowardsTarget(fsm.enemy.statHandler.data.spd, fsm.enemy.statHandler.data.atkRange, fsm.AttackState);
    }
}
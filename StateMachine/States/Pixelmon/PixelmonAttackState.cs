using UnityEngine;

public class PixelmonAttackState : AttackState
{
    private new PixelmonFSM fsm;
    public PixelmonAttackState(PixelmonFSM fsm) 
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        base.Enter();
        fsm.InvokeAttack(true);
    }

    public override void Exit()
    {
        base.Exit();
        fsm.InvokeAttack(false);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDieState : DieState
{
    private new PlayerFSM fsm;
    public PlayerDieState(PlayerFSM fsm) 
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        Player.Instance.ChangePixelmonsState(PixelmonState.Idle);
        fsm.rb.velocity = Vector2.zero;
        GameManager.Instance.NotifyPlayerDie();
        base.Enter();
    }
}

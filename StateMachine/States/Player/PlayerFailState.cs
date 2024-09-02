using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFailState : FailState
{
    private new PlayerFSM fsm;
    public PlayerFailState(PlayerFSM fsm)
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        Player.Instance.ChangePixelmonsState(PixelmonState.Idle);
        //fsm.joystick.gameObject.SetActive(false);
        fsm.rb.velocity = Vector2.zero;
        base.Enter();
    }
}

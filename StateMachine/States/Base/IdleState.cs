using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(FSM fsm)
        : base(fsm)
    {
    }

    public override void Enter()
    {
        StartAnimation(fsm.animData.IdleParameterHash);
    }

    public override void Execute()
    {
        fsm.Flip();
    }

    public override void Exit()
    {
        StopAnimation(fsm.animData.IdleParameterHash);
    }
}

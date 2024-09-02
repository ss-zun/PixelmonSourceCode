using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailState : BaseState
{
    public FailState(FSM fsm)
        : base(fsm)
    {
    }

    public override void Enter()
    {
        StartAnimation(fsm.animData.FailParameterHash);
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
        StopAnimation(fsm.animData.FailParameterHash);
    }
}

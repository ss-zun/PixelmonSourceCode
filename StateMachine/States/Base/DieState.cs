using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : BaseState
{
    public DieState(FSM fsm) 
        : base(fsm)
    {
    }

    public override void Enter()
    {
        StartAnimation(fsm.animData.DieParameterHash);
        fsm.rb.velocity = Vector3.zero;
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {    
        StopAnimation(fsm.animData.DieParameterHash);
    }
}

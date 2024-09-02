using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FSM stateMachine) 
        : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StartAnimation(fsm.animData.AttackParameterHash);
    }

    public override void Execute()
    {
        fsm.Flip();
    }

    public override void Exit()
    {
        StopAnimation(fsm.animData.AttackParameterHash);
    }
}

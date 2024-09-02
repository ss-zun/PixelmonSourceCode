using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PixelmonMoveState : MoveState
{
    private new PixelmonFSM fsm;
    public PixelmonMoveState(PixelmonFSM fsm) 
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Execute()
    {
        // 플레이어 입력에 따라 이동
        if (Player.Instance.fsm.isActiveMove)
        {
            if (Player.Instance.fsm.MovementInput.x < 0)
            {
                fsm.pixelmon.GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            else
            {
                fsm.pixelmon.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
        }
        else
        {
            base.Execute();
        }
    }
}

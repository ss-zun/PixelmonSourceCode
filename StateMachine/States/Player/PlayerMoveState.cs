using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveState : MoveState
{
    private new PlayerFSM fsm;
    public PlayerMoveState(PlayerFSM fsm)
        :base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        base.Enter();
        Player.Instance.ChangePixelmonsState(PixelmonState.Move);
    }

    public override void Execute()
    {
        // 플레이어 입력에 따라 이동
        if (fsm.isActiveMove)
        {
            fsm.rb.velocity = fsm.MovementInput * Player.Instance.statHandler.data.baseSpd;
        }
        // 입력이 없을 경우 타겟을 향해 이동
        else
        {
            base.Execute();
            MoveTowardsTarget(Player.Instance.statHandler.data.baseSpd, 
                Player.Instance.statHandler.data.baseAtkRange, fsm.AttackState);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _statemachine, string _animBoolName) : base(_player, _statemachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //player.skill.clone.CreateCloneOnStart(player.transform, Vector2.zero);
        player.skill.clone.CreateCloneOnDashStart();

        stateTimer = player.dashDuration;
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        //player.skill.clone.CreateCloneOnEnd(player.transform, Vector2.zero);
        player.skill.clone.CreateCloneOnDashEnd();

        player.SetVelocity(0, rb.velocity.y);
    }
}

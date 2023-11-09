using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _statemachine, string _animBoolName) : base(_player, _statemachine, _animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        PlayerManager.instance.player.HideSprite(false); // Needed this code for a bug; when pressing RTrigger after ultimate, player would disappear (ReleaseCloneAttack would trigger before FinishBlackHoleAbility
    }

    public override void Update()
    {
        base.Update();

        if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Joystick1Button4)) && HasNoSword())
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            stateMachine.ChangeState(player.counterAttack);

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            stateMachine.ChangeState(player.primaryAttack);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);

        if (InputUtility.RTriggerPulled) // || Input.GetKeyDown(KeyCode.R)
            stateMachine.ChangeState(player.blackHole);
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Skill_Sword_Controller>().ReturnSword();
        return false;
    }

    public override void Exit()
    {
        base.Exit();
    }
}

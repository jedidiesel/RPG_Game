using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _statemachine, string _animBoolName) : base(_player, _statemachine, _animBoolName)
    {
    }

    
    public override void Enter()
    {
        base.Enter();

        player.skill.sword.DotsActive(true); // Activate dots prefab when entering the aim sword state
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity(); // Disallow movement while aiming

        if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Joystick1Button4))
            stateMachine.ChangeState(player.idleState);

        // Ensure the player is facing the direction of sword throw trajectory
        float rAnalogPosition = Input.GetAxisRaw("RHorizontal");
        float threshold = 0.1f;

        // Commented below in place of code below to allow for mouse input
        //if (rAnalogPosition + threshold > .01 && player.facingDir == -1)
        //    player.Flip();
        //else if (rAnalogPosition + threshold < .01 && player.facingDir == 1)
        //    player.Flip();

        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            if (rAnalogPosition + threshold > .01 && player.facingDir == -1)
                player.Flip();
            else if (rAnalogPosition + threshold < .01 && player.facingDir == 1)
                player.Flip();
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
            player.Flip();
        }
         
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.2f);  // Slight time delay to disallow movement of player immediately after throwing the sword
    }
}

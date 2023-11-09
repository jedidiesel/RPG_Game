using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _statemachine, string _animBoolName) : base(_player, _statemachine, _animBoolName)
    {
    }

    
    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        //Flip player to face direction of returning sword
        if (player.transform.position.x > sword.position.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1)
            player.Flip();

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y); // Causes slight velocity (impact) upon catching the sword
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState); // After animation trigger called, move to idlestate
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.1f); // Create slight delay on movement upon exit
    }    
}

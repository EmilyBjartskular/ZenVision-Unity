using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveState : StateManager
{
    public ActiveState(State current, StateManager next) : base(current, next)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}

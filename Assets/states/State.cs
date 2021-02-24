using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State { 
    IDLE, CLOSE, OPEN, ACTIVE
}
public enum Action {
    UPDATE, ENTER, EXIT
}
public abstract class StateManager
{

    
    private State current;
    private Action action;
    private StateManager next;
    public StateManager(State current, StateManager next) {
        this.current = current;
        this.next = next;
        this.action = Action.ENTER;
    }
    public StateManager Process() {
        switch (this.action) {
            case Action.ENTER:
                Enter();
                break;
            case Action.EXIT:
                Exit();
                return next;
            case Action.UPDATE:
                Update();
                break;
        }
        return this;
    }

    public virtual void Update() { }
    public virtual void Enter() {
        this.action = Action.UPDATE;
    }
    public virtual void Exit() { }


}

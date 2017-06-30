using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class StatefulObjectBase<T, TEnum> : MonoBehaviour
    where T : class where TEnum : System.IConvertible
{
    protected List<State<T>> stateList = new List<State<T>>();

   
    protected StateMachine<T> stateMachine;
    public bool isBassActive=true;
    public virtual void ChangeState(TEnum state)
    {
        if (stateMachine == null)
        {
            return;
        }

        stateMachine.ChangeState(stateList[state.ToInt32(null)]);
    }

    public virtual bool IsCurrentState(TEnum state)
    {
        if (stateMachine == null)
        {
            return false;
        }

        return stateMachine.CurrentState == stateList[state.ToInt32(null)];
    }
    protected virtual void Update()
    {
        if (stateMachine != null)
        {
            if(isBassActive)stateMachine.Update();
        }
    }
   
}
public class StateMachine<T>
{
    private State<T> currentState;

    public StateMachine()
    {
        currentState = null;
    }

    public State<T> CurrentState
    {
        get { return currentState; }
    }

    public void ChangeState(State<T> state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }
    public void Update()
    {
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SingletonStatefulObjectBase<T, TEnum> : MonoBehaviour
    where T : class where TEnum : System.IConvertible
{
   


    protected List<Mode<T>> stateList = new List<Mode<T>>();


    protected StateMachineMode<T> stateMachine;
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
        
    }

}
public class Mode<T>
{
    // このステートを利用するインスタンス
    protected T owner;

    public Mode(T owner)
    {
        this.owner = owner;
    }

    // このステートに遷移する時に一度だけ呼ばれる
    public virtual void Enter() {}
    public virtual void Exit() {}
}
public class StateMachineMode<T>
{
    private Mode<T> currentState;

    public StateMachineMode()
    {
        currentState = null;
    }

    public Mode<T> CurrentState
    {
        get { return currentState; }
    }

    public void ChangeState(Mode<T> state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }

}


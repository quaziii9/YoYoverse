using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _State
{
    Idle,
    Move,
    ComboAttack1,
    ComboAttack2,
    ComboAttack3
}

public class PlayerStateMachine : MonoBehaviour
{
    private Dictionary<_State, BaseState> _stateDictionary = new Dictionary<_State, BaseState>();
    private BaseState _currentState;
    
    void Start()
    {
        _currentState = _stateDictionary[_State.Idle];

        _currentState.StateEnter();
    }

    void Update()
    {
        _currentState.StateUpdate();
    }

    public void AddState(_State state, BaseState newState)
    {
        _stateDictionary.Add(state, newState); 
    }

    public void ChangeState(_State newState)
    {
        _currentState.StateExit();

        _currentState = _stateDictionary[newState];

        _currentState.StateEnter();
    }
}

public abstract class BaseState
{
    public virtual void StateEnter() { }
    public virtual void StateUpdate() { }
    public virtual void StateExit() { }
}
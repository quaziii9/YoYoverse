using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Move,
    Attack,

}

public class PlayerStateMachine : MonoBehaviour
{
    private Dictionary<PlayerState, BaseState> _stateDictionary = new Dictionary<PlayerState, BaseState>();
    private BaseState _currentState;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public abstract class BaseState
{

}
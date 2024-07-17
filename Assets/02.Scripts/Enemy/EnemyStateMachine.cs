using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private Dictionary<EnemyState, EnemyBase> _stateDic = new Dictionary<EnemyState, EnemyBase>();
    private EnemyBase _currentState;

    void Start()
    {
        _currentState = _stateDic[EnemyState.Idle];

        _currentState.StateEnter();
    }

    
    void Update()
    {
        _currentState.StateUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        _currentState.OnTriggerEnter(other);
    }

    public void AddState(EnemyState newState, EnemyBase stateComponent)
    {
        _stateDic.Add(newState, stateComponent);
    }

    public void ChangeState(EnemyState newState)
    {
        _currentState.StateExit();

        _currentState = _stateDic[newState];

        _currentState.StateEnter();
    }
}

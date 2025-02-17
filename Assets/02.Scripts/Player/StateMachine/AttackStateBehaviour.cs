using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    [Header("ResetTrigger")]
    [SerializeField] private string _attack;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_attack);
    }

}

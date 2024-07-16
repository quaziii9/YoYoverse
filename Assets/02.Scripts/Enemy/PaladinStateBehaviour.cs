using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinStateBehaviour : StateMachineBehaviour
{
    Paladin _paladin;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_paladin == null)
        {
            _paladin = animator.GetComponent<Paladin>();
        }

        _paladin.IsAction = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");

        _paladin.IsAction = false;
    }
}

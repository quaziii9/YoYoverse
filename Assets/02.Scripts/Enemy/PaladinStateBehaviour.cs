using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinStateBehaviour : StateMachineBehaviour
{
    Paladin _paladin;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_paladin == null)
        {
            _paladin = animator.GetComponent<Paladin>();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");

        if(Vector3.Distance(_paladin.transform.position, _paladin.Player.transform.position) > 1.0f)
        {
            _paladin.State.ChangeState(EnemyState.Trace);
        }
        else
            _paladin.State.ChangeState(EnemyState.Attack);
    }
}

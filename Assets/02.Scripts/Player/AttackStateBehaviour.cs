using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    [Header("ResetTrigger")]
    [SerializeField] private string _attack;

    private PlayerController _moveController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetComponent(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _moveController.IsMove = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_attack);
        _moveController.IsMove = true;
    }

    private void GetComponent(Animator animator)
    {
        if (_moveController == null)
        {
            _moveController = animator.GetComponent<PlayerController>();
        }
    }
}

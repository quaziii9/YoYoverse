using UnityEngine;

public class AssassinationState : PlayerState
{
    public AssassinationState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Anim.SetBool(_player.IsSkillAssassination, true);
    }

    public override void StateUpdate()
    {

    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsSkillAssassination, false);
    }
}

public class DefenseState : PlayerState
{
    public DefenseState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Anim.SetBool(_player.IsSkillDefense, true);
        // 요요 크기 늘리기
    }

    public override void StateUpdate()
    {
        // Z 키를 떼면 Idle 상태로 전환
        if (Input.GetKeyUp(KeyCode.Z))
        {
            _player.PlayerStateMachine.ChangeState(State.Idle);
        }
    }

    public override void StateExit()
    {
        // 요요 크기 줄이기
        _player.Anim.SetBool(_player.IsSkillDefense, false);
    }
}
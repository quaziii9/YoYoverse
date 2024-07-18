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
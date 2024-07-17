using UnityEngine;

public abstract class PlayerState : BaseState
{
    protected Player _player;
    public PlayerState(Player player)
    {
        _player = player;
    }

    protected void AttackRotation()
    {
        _player.MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, 100, _player.Mask, QueryTriggerInteraction.Ignore))
        {
            Vector3 lookPosition = new Vector3(hit.point.x, _player.transform.position.y, hit.point.z);

            float distance = Vector3.Distance(_player.transform.position, hit.point);

            if (distance > 0.1f)
            {
                _player.transform.LookAt(lookPosition);
            }
        }
    }

}

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Agent.enabled = false;
        // _player.Anim.applyRootMotion = true;
    }

    public override void StateUpdate()
    {
        ChangeMove();
        ChangeAttack();
    }

    public override void StateExit()
    {
        _player.Agent.enabled = true;
        _player.Anim.applyRootMotion = false;
    }

    private void ChangeMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.MouseRay = _player.MainCamera.ScreenPointToRay(Input.mousePosition);

            _player.State.ChangeState(_State.Move);
        }
    }

    private void ChangeAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttackRotation();
            _player.State.ChangeState(_State.ComboAttack1);
        }
    }
}

public class MoveState : PlayerState
{
    public MoveState(Player player) : base(player) { }

    //상태 진입.
    public override void StateEnter()
    {
        _player.Agent.isStopped = false;

        RayCast();
    }

    //현재 상태 업데이트.
    public override void StateUpdate()
    {
        ClickMove();
    }

    //상태 종료.
    public override void StateExit()
    {
        _player.Agent.SetDestination(_player.transform.position);
    }

    //이동처리 메소드
    private void ClickMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.MouseRay = _player.MainCamera.ScreenPointToRay(Input.mousePosition);

            RayCast();
        }
        else if (_player.Agent.remainingDistance < 0.1f)
        {
            ActiveTargetObject(false);

            _player.State.ChangeState(_State.Idle);
        }

        AnimationMoveMent();
        ChangeAttack();
    }

    //레이캐스트로 이동 방향을 설정하는 메소드
    private void RayCast()
    {
        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, Mathf.Infinity, _player.Mask, QueryTriggerInteraction.Ignore))
        {
            _player.Agent.SetDestination(hit.point);

            _player.ClickObject.position = new Vector3(hit.point.x, 0.01f, hit.point.z);

            ActiveTargetObject(true);
        }
    }

    //타겟 오브젝트 활성화, 비활성화 메소드
    private void ActiveTargetObject(bool isActive)
    {
        _player.ClickObject.gameObject.SetActive(isActive);
    }

    //Agent 속도에 따른 이동 애니메이션 메소드
    private void AnimationMoveMent()
    {
        Vector3 currentVelocity = _player.Agent.velocity;

        float speed = currentVelocity.magnitude;

        _player.Anim.SetFloat("Move", speed);
    }

    private void ChangeAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttackRotation();
            ActiveTargetObject(false);

            _player.Agent.isStopped = true;

            _player.Anim.SetFloat("Move", 0);

            _player.State.ChangeState(_State.ComboAttack1);
        }
    }
}

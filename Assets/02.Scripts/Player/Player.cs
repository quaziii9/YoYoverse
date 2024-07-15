using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("MoveSpeed")]
    [SerializeField] private float walkSpeed;

    [Header("ClickPosition")]
    [SerializeField] private Transform clickTransform;

    #region PlayerComponent
    private NavMeshAgent _agent;
    private Animator _animator;
    private PlayerStateMachine _state;
    #endregion

    #region PlayerValue
    private Ray _mouseRay;
    private Camera _mainCamera;
    private bool isNext;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }    
    public Animator Anim { get { return _animator; } }
    public PlayerStateMachine State { get { return _state; } }
    public Transform ClickObject { get { return clickTransform; } }
    public Camera MainCamera { get {  return _mainCamera; } }
    public Ray MouseRay { get { return _mouseRay; } set { _mouseRay = value; } }
    public bool IsNext { get { return isNext; } set { isNext = value; } }
    #endregion

    #region Animation 
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    #endregion

    private void Awake()
    {
        InitializePlayer();
        InitializeState();
    }

    private void InitializePlayer()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;
        _agent.speed = walkSpeed;
        clickTransform.gameObject.SetActive(false);
    }

    private void InitializeState()
    {
        _state = gameObject.AddComponent<PlayerStateMachine>();
        _state.AddState(_State.Idle, new IdleState(this));
        _state.AddState(_State.Move , new MoveState(this));  
        _state.AddState(_State.ComboAttack1, new FirstAttackState(this));
        _state.AddState(_State.ComboAttack2, new SecondAttackState(this));
        _state.AddState(_State.ComboAttack3, new ThirdAttackState(this));  
    }

    public void ChangeNextAttack()
    {
        isNext = true;
    }
}

public abstract class PlayerState : BaseState
{
    protected Player _player;
    public PlayerState(Player player)
    {
        _player = player;
    }
}

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Agent.enabled = false;
        _player.Anim.applyRootMotion = true;
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
    }

    //레이캐스트로 이동 방향을 설정하는 메소드
    private void RayCast()
    {
        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, Mathf.Infinity))
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
}

public class FirstAttackState : PlayerState
{
    public FirstAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Agent.enabled = false;

        _player.Anim.applyRootMotion = true;

        _player.IsNext = false;

        _player.Anim.SetBool(_player.IsComboAttack1, true);
    }

    public override void StateUpdate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if(animatorStateInfo.IsName("attack01") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.Anim.SetTrigger("ComboFail");
            _player.State.ChangeState(_State.Idle);
        }

        if(Input.GetMouseButtonDown(0) && _player.IsNext)
        {
            _player.State.ChangeState(_State.ComboAttack2);
        }
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack1, false);
    }

    //레이 방향으로 회전하는 메소드
    private void AttackRotation()
    {
        _player.MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, 100))
        {
            Vector3 lookPosition = new Vector3(hit.point.x, _player.transform.position.y, hit.point.z);

            float distance = Vector3.Distance(_player.transform.position, hit.point);

            if(distance > 0.1f)
            {
                _player.transform.LookAt(lookPosition);
            }
        }
    }
}

public class SecondAttackState : PlayerState
{
    public SecondAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Anim.SetBool(_player.IsComboAttack2, true);

        _player.IsNext = false;
    }

    public override void StateUpdate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if(animatorStateInfo.IsName("attack02") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.Anim.SetTrigger("ComboFail");
            _player.State.ChangeState(_State.Idle);
        }

        if(Input.GetMouseButtonDown(0) && _player.IsNext)
        {
            _player.State.ChangeState(_State.ComboAttack3);
        }
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack2, false);
    }

}

public class ThirdAttackState : PlayerState
{
    public ThirdAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Anim.SetBool(_player.IsComboAttack3, true);

        _player.IsNext = false;
    }

    public override void StateUpdate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if(animatorStateInfo.IsName("attack03") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.State.ChangeState(_State.Idle);
        }
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack3, false);
    }

}
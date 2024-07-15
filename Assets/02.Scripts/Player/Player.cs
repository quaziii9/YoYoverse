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
    private bool isMove;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }    
    public Animator Anim { get { return _animator; } }
    public PlayerStateMachine State { get { return _state; } }
    public Transform ClickObject { get { return clickTransform; } }
    public Camera MainCamera { get {  return _mainCamera; } }
    public Ray MouseRay { get { return _mouseRay; } set { _mouseRay = value; } }
    public bool IsMove { get { return isMove; } }
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
}

public class MoveState : PlayerState
{
    public MoveState(Player player) : base(player) { }

    public override void StateEnter()
    {
        RayCast();
    }

    public override void StateUpdate()
    {
        ClickMove();
    }

    public override void StateExit()
    {
     
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



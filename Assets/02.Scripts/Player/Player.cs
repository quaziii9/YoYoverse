using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("id")]
    [SerializeField] private int id;

    [Header("MoveSpeed")]
    [SerializeField] private float walkSpeed;

    [Header("ClickPosition")]
    [SerializeField] private Transform clickTransform;

    #region PlayerComponent
    private NavMeshAgent _agent;
    private Animator _animator;
    private PlayerStateMachine _state;
    private Rigidbody _rigidBody;
    #endregion

    #region PlayerValue
    private Ray _mouseRay;
    private LayerMask _layerMask;
    private Camera _mainCamera;
    private float _power;
    private float _attackSpeed;
    private float _defense;
    private float _moveSpeed;
    private bool isNext;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }    
    public Animator Anim { get { return _animator; } }
    public PlayerStateMachine State { get { return _state; } }
    public Transform ClickObject { get { return clickTransform; } }
    public Camera MainCamera { get {  return _mainCamera; } }
    public Rigidbody RigidBody { get { return _rigidBody; } }   
    public LayerMask Mask { get { return _layerMask; } }
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

    //플레이어 초기화
    private void InitializePlayer()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _agent.speed = walkSpeed;
        _layerMask = LayerMask.GetMask("Ground");
        clickTransform.gameObject.SetActive(false);
    }

    //상태 추가 메소드 
    private void InitializeState()
    {
        _state = gameObject.AddComponent<PlayerStateMachine>();
        _state.AddState(_State.Idle, new IdleState(this));
        _state.AddState(_State.Move , new MoveState(this));  
        _state.AddState(_State.ComboAttack1, new FirstAttackState(this));
        _state.AddState(_State.ComboAttack2, new SecondAttackState(this));
        _state.AddState(_State.ComboAttack3, new ThirdAttackState(this));  
    }

    //애니메이션 이벤트
    public void ChangeNextAttack()
    {
        isNext = true;
    }
}
using EnumTypes;
using EventLibrary;
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

    [Header("FirstAttackParticle")]
    [SerializeField] private GameObject _firstParticle;

    [Header("SecondAttackParticle")]
    [SerializeField] private GameObject _secondParticle;

    [Header("ThirdAttackParticle")]
    [SerializeField] private GameObject _thirdParticle;

    #region PlayerComponent
    private NavMeshAgent _agent;
    private ParticleSystem _first, _second, _third;
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
        InitializeEffect();
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

    private void InitializeEffect()
    {
        _first = _firstParticle.GetComponent<ParticleSystem>();
        _second = _secondParticle.GetComponent<ParticleSystem>();
        _third = _thirdParticle.GetComponent<ParticleSystem>();
    }

    //상태 추가 메소드 
    private void InitializeState()
    {
        _state = gameObject.AddComponent<PlayerStateMachine>();
        _state.AddState(global::State.Idle, new IdleState(this));
        _state.AddState(global::State.Move , new MoveState(this));  
        _state.AddState(global::State.ComboAttack1, new FirstAttackState(this));
        _state.AddState(global::State.ComboAttack2, new SecondAttackState(this));
        _state.AddState(global::State.ComboAttack3, new ThirdAttackState(this));  
    }

    //애니메이션 이벤트
    public void ChangeNextAttack()
    {
        isNext = true;
    }

    public void FirstAttackParticle()
    {
        _first.Play();
        OnEffect effectComponent = _firstParticle.GetComponent<OnEffect>();
        effectComponent.TriggerEvent(1);
    }

    public void SecondAttackParticle()
    {
        _second.Play();
        OnEffect effectComponent = _secondParticle.GetComponent<OnEffect>();
        effectComponent.TriggerEvent(2);
    }

    public void ThirdAttackParticle()
    {
        _third.Play();
        OnEffect effectComponent = _thirdParticle.GetComponent<OnEffect>();
        effectComponent.TriggerEvent(3);
    }
}
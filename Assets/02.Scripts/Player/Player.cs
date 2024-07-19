using System.Collections;
using System.Net.NetworkInformation;
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

    // 암살 타겟 적할당
    [Header("AssassinationTargetEnemey")]
    [SerializeField] private EnemyAI _assinationTargetEnemy;

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
    private float _power = 1f;
    private float _attackSpeed;
    private float _defense;
    private float _moveSpeed;
    private bool isNext;
    private bool isDead = false;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }
    public Animator Anim { get { return _animator; } }
    public PlayerStateMachine PlayerStateMachine { get { return _state; } }
    public Transform ClickObject { get { return clickTransform; } }
    public Camera MainCamera { get { return _mainCamera; } }
    public Rigidbody RigidBody { get { return _rigidBody; } }
    public LayerMask Mask { get { return _layerMask; } }
    public Ray MouseRay { get { return _mouseRay; } set { _mouseRay = value; } }
    public bool IsNext { get { return isNext; } set { isNext = value; } }
    public bool IsDead 
    { 
        get { return isDead; }

        set
        {
            isDead = value;

            if (isDead)
            {
                _state.ChangeState(global::State.Die);
            }
        }
    }
    #endregion

    #region Animation 
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    public readonly int DieParam = Animator.StringToHash("Die");

    public readonly int IsSkillAssassination = Animator.StringToHash("IsSkillAssassination");
    public readonly int IsSkillDefense = Animator.StringToHash("IsSkillDefense");
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
        _state.AddState(global::State.Move, new MoveState(this));
        _state.AddState(global::State.ComboAttack1, new FirstAttackState(this));
        _state.AddState(global::State.ComboAttack2, new SecondAttackState(this));
        _state.AddState(global::State.ComboAttack3, new ThirdAttackState(this));
        _state.AddState(global::State.SkillDefense, new DefenseState(this));
        _state.AddState(global::State.SkillAssassination, new AssassinationState(this));
        _state.AddState(global::State.Die, new DieState(this)); 
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
        effectComponent.SetPower(_power);
        effectComponent.AttackEvent(1);
    }

    public void SecondAttackParticle()
    {
        _second.Play();
        OnEffect effectComponent = _secondParticle.GetComponent<OnEffect>();
        effectComponent.SetPower(_power);
        effectComponent.AttackEvent(2);
    }

    public void ThirdAttackParticle()
    {
        _third.Play();
        OnEffect effectComponent = _thirdParticle.GetComponent<OnEffect>();
        StartCoroutine(ThirdAttackDelay(effectComponent));
    }

    //3타 딜레이
    private IEnumerator ThirdAttackDelay(OnEffect effectComponenet)
    {
        yield return new WaitForSeconds(0.6f);

        effectComponenet.SetPower(_power);
        effectComponenet.AttackEvent(3);
    }


    // 적의 backcollider에 닿았을시 _assinationTargetEnemy에 해당 적 할당
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBack"))
        {
            _assinationTargetEnemy = other.GetComponentInParent<EnemyAI>();
        }

        // 디펜스중 적 무기 맞으면 idlestate로 
       if ((PlayerStateMachine._currentState == PlayerStateMachine._stateDictionary[State.SkillDefense]) &&
          (other.CompareTag("EnemyWeapon") || other.CompareTag("Bullet")))
        {
            PlayerStateMachine.ChangeState(State.Idle);
        }
    }

    // 적의 backcollider에서 나갔을시 _assinationTargetEnemy에 null 할당
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyBack"))
        {
            _assinationTargetEnemy = null;
        }
    }

    // 암살 시도 시 _assinationTargetEnemy에 null 할당
    public void TryAssassinate()
    {
        if (_assinationTargetEnemy != null)
        {
            _assinationTargetEnemy.BeAssassinate();
            _assinationTargetEnemy = null;
        }
    }

    private void AssassinationAnimationEnd()
    {
        PlayerStateMachine.ChangeState(State.Idle);
    }
}
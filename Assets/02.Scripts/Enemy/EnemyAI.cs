using System;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public IState CurrentStateInstance { get; private set; }
    public EnemyState EnemyCurstate = EnemyState.Idle;
    public Transform enemyTr;
    public Transform playerTr;
    public Animator animator;
    public EnemyFire enemyFire;
    public EnemyHealth enemyHealth;

    public bool isDie = false;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시 값을 미리 추출
    public readonly int Idle = Animator.StringToHash("IsIdle");
    public readonly int HashDie = Animator.StringToHash("IsDie");
    public readonly int HashAssassinationDie = Animator.StringToHash("IsAssassinationDie");

    // public Vector3 initialPosition { get; private set; }
    public Quaternion initialRotation { get; private set; }

    //public float modelRotationOffset = 30f; // 모델의 회전 오프셋
    
    [Header("SniperRotattionAngle")]
    public int rotationAngle;

    [Header("Tracetime")]
    public float maxTraceTime = 5f;
    public float currentTraceTimer = 0f;

    [Header("First Attack State RotationSpped & FireDelayTime")]
    public float initialRotationSpeed = 5f;
    public float initialFireDelay = 1f;

    [Header("First Attack After RotationSpped & FireDelayTime")]
    public float continuousRotationSpeed = 10f;
    public float continuousFireDelay = 2f;

    private void Awake()
    {
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();

       // initialPosition = enemyTr.position;
        initialRotation = enemyTr.rotation;
    }

    private void OnEnable()
    {
        EventManager<EnemyEvents>.StartListening(EnemyEvents.PlayerDetected, OnPlayerDetected);
        EventManager<EnemyEvents>.StartListening(EnemyEvents.PlayerLost, OnPlayerLost);
    }

    private void OnDisable()
    {
        EventManager<EnemyEvents>.StopListening(EnemyEvents.PlayerDetected, OnPlayerDetected);
        EventManager<EnemyEvents>.StopListening(EnemyEvents.PlayerLost, OnPlayerLost);
    }


    private void OnPlayerDetected()
    {
        if (EnemyCurstate != EnemyState.Attack)
        {
            ChangeState(EnemyState.Attack);
        }
        currentTraceTimer = 0f;
    }

    private void OnPlayerLost()
    {
        if (EnemyCurstate == EnemyState.Attack)
        {
            ChangeState(EnemyState.Trace);
        }
    }


    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        ChangeState(EnemyState.Idle);
    }


    private void Update()
    {
        CurrentStateInstance?.ExecuteOnUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentStateInstance?.Exit(); // 현재 상태 종료

        EnemyCurstate = newState;

        CurrentStateInstance = CreateStateInstance(newState); // 새로운 상태 인스턴스 생성
        CurrentStateInstance?.Enter(); // 새로운 상태 진입
    }

    private IState CreateStateInstance(EnemyState enemyState)
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                return new EnemyIdleState(this);
            case EnemyState.Attack:
                return new EnemyAttackState(this, initialRotationSpeed, initialFireDelay,
                                            continuousRotationSpeed, continuousFireDelay);
            case EnemyState.Trace:
                return new EnemyTraceState(this);
            case EnemyState.Die:
                return new EnemyDieState(this);
            case EnemyState.AssassinationDie:
               return new EnemyAssassinationDieState(this);
            default:
                throw new System.ArgumentOutOfRangeException(nameof(enemyState), enemyState, null);
        }
    }

    public void ResetToInitialTransform()
    {
       // enemyTr.position = initialPosition;
        enemyTr.rotation = initialRotation;
    }

    public void BeAssassinate()
    {
        ChangeState(EnemyState.AssassinationDie);
        enemyHealth.BeAssassinateDamage();
    }



}
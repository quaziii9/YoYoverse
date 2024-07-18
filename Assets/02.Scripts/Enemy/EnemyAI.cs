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

    public int rotationAngle;
    public bool isDie = false;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시 값을 미리 추출
    public readonly int Idle = Animator.StringToHash("IsIdle");
    public readonly int HashDie = Animator.StringToHash("IsDie");
    public readonly int HashAssassinationDie = Animator.StringToHash("IsAssassinationDie");

    public float modelRotationOffset = 30f; // 모델의 회전 오프셋
   // public Vector3 initialPosition { get; private set; }
    public Quaternion initialRotation { get; private set; }

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
        EventManager<EnemyEvents>.StartListening(EnemyEvents.ChangeEnemyStateAttack, ChangeAttackState);
    }

    private void OnDisable()
    {
        EventManager<EnemyEvents>.StopListening(EnemyEvents.ChangeEnemyStateAttack, ChangeAttackState);
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
            case EnemyState.Patrol:
                return new EnemyPatrolState(this);
            case EnemyState.Trace:
                return new EnemyTraceState(this);
            case EnemyState.Attack:
                return new EnemyAttackState(this);
            case EnemyState.Die:
                return new EnemyDieState(this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // 적 감지 EventManager로 불러올 함수 
    private void ChangeAttackState()
    {
        ChangeState(EnemyState.Attack);
    }

    public void ResetToInitialTransform()
    {
       // enemyTr.position = initialPosition;
        enemyTr.rotation = initialRotation;
    }

    public void BeAssassinate()
    {
        enemyHealth.BeAssassinateDamage();
    }

}
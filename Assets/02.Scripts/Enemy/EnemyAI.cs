using UnityEngine;
using System;
using EventLibrary;
using EnumTypes;


public enum EnemyState
{
    Idle,
    PATROL,
    TRACE,
    ATTACK,
    DIE
}

public enum EnemyType
{
    Sniper,
    Paladin
}

public class EnemyAI : MonoBehaviour
{

    public IState CurrentStateInstance { get; private set; }
    public EnemyState EnemyCurstate = EnemyState.Idle;
    public EnemyType EnemyType;
    public int RotationAngle;

    public Transform playerTr;
    public Transform enemyTr;

    public Animator animator;
    public EnemyMoveAgent moveAgent;
    public EnemyFire enemyFire;

    public float attackDist = 8.0f; // 공격 거리
    public float traceDis = 15.0f;  // 쫓아가는 거리
    public float staticTraceDis = 15.0f;    // 고정 거리
    public bool isDie = false;

    private float playerHp;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시 값을 미리 추출
    public readonly int Idle = Animator.StringToHash("IsIdle");
    public readonly int hashMove = Animator.StringToHash("IsMove");
    public readonly int hashSpeed = Animator.StringToHash("Speed");
    public readonly int hashDie = Animator.StringToHash("IsDie");
    public readonly int hashOffeset = Animator.StringToHash("Offset");
    public readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //if (player != null) playerTr = player.GetComponent<Transform>();

        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        moveAgent = GetComponent<EnemyMoveAgent>();
        enemyFire = GetComponent<EnemyFire>();
    }

    private void OnEnable()
    {
        EventManager<EnemyEvents>.StartListening(EnemyEvents.ChangeEnemyStateAttack, ChangeAttackState);
    }

    private void OnDisable()
    {
        EventManager<EnemyEvents>.StopListening(EnemyEvents.ChangeEnemyStateAttack, ChangeAttackState);
    }

    public void Start()
    {
        ChangeState(EnemyState.Idle);
    }

    public void Update()
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
        switch(enemyState)
        {
            case EnemyState.Idle:
                return new IdleState(this);
            case EnemyState.PATROL:
                return new PatrolState(this);
            case EnemyState.TRACE:
                return new TraceState(this);
            case EnemyState.ATTACK:
                    return new AttackState(this);
            case EnemyState.DIE:
                return new DieState(this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // 적감지 eventmanager로 불러올 함수 
    private void ChangeAttackState()
    {
        ChangeState(EnemyState.ATTACK);
    }

    //IEnumerator CheckState()
    //{
    //    while (!isDie)
    //    {
    //        //var player = FindObjectOfType<PlayerController>();
    //        ////Debug.Log(player);
    //        //if (player != null) playerTr = player.GetComponent<Transform>();
    //        ////Debug.Log(playerTr.position);
    //        //if (state == State.DIE) yield break;

    //        //playerHp = player.GetComponentInParent<Status>().currentHp;
    //        if (playerHp <= 0)
    //        {
    //            state = State.PATROL;
    //            yield break;
    //        }
    //        float dist = Vector3.Distance(playerTr.position, enemyTr.position);

    //        if (dist <= attackDist) state = State.ATTACK;
    //        else if (dist <= traceDis) state = State.TRACE;
    //        else state = State.PATROL;

    //    }
    //}

    // void change()
    //{
    //    // 적 캐릭터 사망할 때까지 무한루프
    //    while (!isDie)
    //    {
    //        switch (state)
    //        {
    //            case State.PATROL:
    //                // 총알 발사 정지
    //                enemyFire.isFire = false;
    //                moveAgent.patrolling = true;
    //                animator.SetBool(hashMove, true);
    //                break;
    //            case State.TRACE:
    //                enemyFire.isFire = false;
    //                moveAgent.traceTarget = playerTr.position;
    //                animator.SetBool(hashMove, true);
    //                break;
    //            case State.ATTACK:
    //                traceDis = staticTraceDis;
    //                moveAgent.Stop();
    //                animator.SetBool(hashMove, false);

    //                if (enemyFire.isFire == false) enemyFire.isFire = true;
    //                break;
    //            case State.DIE:
    //                isDie = true;
    //                enemyFire.isFire = false;
    //                moveAgent.Stop();

    //                // 모든 자식 오브젝트의 콜라이더 비활성화
    //                foreach (Collider collider in childColliders)
    //                {
    //                    collider.enabled = false;
    //                }
    //                break;
    //        }
    //    }
    //}

}

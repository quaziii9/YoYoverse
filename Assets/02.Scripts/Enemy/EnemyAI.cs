using System.Collections;
using UnityEngine;

public enum State
{
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


    public State state = State.PATROL;

    [Header("Transform")]
    public Transform playerTr;
    public Transform enemyTr;

    public Animator animator;
    public EnemyMoveAgent moveAgent;
    public EnemyFire enemyFire;

    public float attackDist = 8.0f; // 공격 거리
    public float traceDis = 15.0f;  // 쫓아가는 거리
    public float staticTraceDis = 15.0f;    // 고정 거리
    public bool isDie = false;

    private WaitForSeconds ws; // 코루틴 지연시간 변수

    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material changeMaterial;
    public float changeMaterialTime = 1f;

    public Collider[] childColliders;

    public float playerHp;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시 값을 미리 추출
    public readonly int hashMove = Animator.StringToHash("IsMove");
    public readonly int hashSpeed = Animator.StringToHash("Speed");
    public readonly int hashDie = Animator.StringToHash("IsDie");
    public readonly int hashOffeset = Animator.StringToHash("Offset");
    public readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    private IEnemyState currentState;

    private void Awake()
    {
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        moveAgent = GetComponent<EnemyMoveAgent>();
        enemyFire = GetComponent<EnemyFire>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        ws = new WaitForSeconds(0.3f);

        animator.SetFloat(hashOffeset, Random.Range(0.0f, 1.0f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));

        childColliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        SetState(new PatrolState());
        StartCoroutine(UpdateState());
    }

    private IEnumerator UpdateState()
    {
        while (!isDie)
        {
            currentState.Execute(this);
            yield return ws;
        }
    }

    public void SetState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        currentState = newState;
        currentState.Enter(this);
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}

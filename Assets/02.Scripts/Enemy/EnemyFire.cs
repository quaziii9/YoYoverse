using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyFire : MonoBehaviour
{
    private Animator animator;
    private Transform playerTr;
    private Transform enemyTr;
    private EnemyAI enemyAI;

    private readonly int hashFire = Animator.StringToHash("Fire");

    private float nextFire = 0f; // 초기 쿨타임 설정
    private readonly float fireRate = 2.0f; // 쿨타임을 2초로 설정

    [SerializeField] private readonly float reloadTime = 3.0f;
    [SerializeField] private readonly int maxBullet = 10;

    public bool isFire = false;
    public bool isFireAnimIng = false;
    private bool pendingIdleState = false; // Idle 상태 전환 대기 플래그

    [Header("Bullet")]
    public GameObject bullet;
    public GameObject bullet_Shell;

    [SerializeField] private Transform firePos;

    [SerializeField] private float detectionRange = 10.0f; // 사정거리 설정

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();

        // 적의 사정거리 감지를 위해 SphereCollider 추가
        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = detectionRange;

        // 플레이어 Transform 가져오기
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isFire && Time.time >= nextFire && !isFireAnimIng)
        {
            Fire();
        }
    }

    private void Fire()
    {
        // 애니메이션 재생
        isFireAnimIng = true;
        animator.SetTrigger(hashFire);
        FireBullet();
    }

    public void FireBullet()
    {
        // 플레이어의 위치를 기준으로 총알의 방향을 설정
        Vector3 direction = (playerTr.position - firePos.position).normalized;

        // 총알 인스턴스 생성
        GameObject bulletInstance = ObjectPool.Instance.DequeueObject(bullet);
        bulletInstance.transform.position = firePos.position;
        bulletInstance.transform.rotation = Quaternion.LookRotation(direction);

        // 총알 발사 로직 추가 (예: Rigidbody 사용)
        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * 500;
        }

        //EffectManager.Instance.FireEffectGenerate(firePos.position, firePos.rotation);
        //gunShot.PlayOneShot(gunShot.clip);
    }

    // 애니메이션 끝났을 때 호출될 메서드
    public void OnFireAnimationEnd()
    {
        // 다음 발사 시간을 현재 시간 + 쿨타임으로 설정
        nextFire = Time.time + fireRate;
        isFireAnimIng = false;

        // 애니메이션이 끝난 후 Idle 상태로 전환
        if (pendingIdleState)
        {
            enemyAI.ChangeState(EnemyState.Idle);
            pendingIdleState = false;
        }
    }

    public void StartFiring()
    {
        isFire = true;
        pendingIdleState = false; // Fire 상태에서 대기하지 않음
    }

    public void StopFiring()
    {
        isFire = false;
        if (isFireAnimIng)
        {
            pendingIdleState = true; // 애니메이션이 실행 중이면 Idle 상태 전환 대기
        }
        else
        {
            enemyAI.ChangeState(EnemyState.Idle); // 바로 Idle 상태로 전환
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopFiring();
        }
    }

    // 기즈모를 사용하여 사정거리를 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 총알 발사 위치와 방향을 시각화
        if (firePos != null && playerTr != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firePos.position, playerTr.position);
            Gizmos.DrawSphere(playerTr.position, 0.2f); // 플레이어 위치에 작은 구 표시
        }
    }
}

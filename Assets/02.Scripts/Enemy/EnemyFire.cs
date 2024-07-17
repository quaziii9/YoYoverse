using System.Collections;
using EnumTypes;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [PropertySpace(0f, 5f)] public Transform firePos; // 총구 포지션
    
    [FoldoutGroup("State")] public bool isFireAnimIng = false; // 현재 발사 중인지 확인하는 변수
    [FoldoutGroup("State")] public bool inPlayer; // 현재 플레이어가 시야각내에 들어왔는지 확인하는 변수
    
    private bool _pendingIdleState = false;

    [FoldoutGroup("Bullet")]public GameObject bullet;
    [FoldoutGroup("Bullet")]public float detectionRange = 10.0f;
    
    private Animator _animator;
    private Transform _playerTr;
    private EnemyAI _enemyAI;

    private readonly int _hashFire = Animator.StringToHash("Fire");

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<EnemyAI>();

        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = detectionRange;

        _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public IEnumerator FireAfterRotation()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기 후 발사
        Fire();
    }

    public void Fire()
    {
        if (isFireAnimIng) return;
        
        isFireAnimIng = true;
        _animator.SetTrigger(_hashFire);
        FireBullet();
    }

    public void FireBullet()
    {
        Vector3 targetPosition = _playerTr.position + new Vector3(0, 1f, 0);
        Vector3 direction = (targetPosition - firePos.position).normalized;
        GameObject bulletInstance = ObjectPool.Instance.DequeueObject(bullet);
        bulletInstance.transform.position = firePos.position;
        bulletInstance.transform.rotation = Quaternion.LookRotation(direction);

        // 총알에 발사자 정보 설정
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetShooter(this.gameObject);
        }

        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * 50;
        }
    }

    public void OnFireAnimationEnd()
    {
        isFireAnimIng = false;

        if (_pendingIdleState)
        {
            _enemyAI.ChangeState(EnemyState.Idle);
            _pendingIdleState = false;
        }
        else if (_enemyAI.EnemyCurstate == EnemyState.Attack && inPlayer == true)
        {
            _enemyAI.StartCoroutine(WaitAndFire());
        }
    }

    private IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(2f);
        Fire();
    }

    public void StopFiring()
    {
        if (isFireAnimIng)
        {
            _pendingIdleState = true;
        }
        else
        {
            if(_enemyAI.EnemyCurstate == EnemyState.Attack) _enemyAI.ChangeState(EnemyState.Idle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopFiring();
            inPlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (firePos != null && _playerTr != null)
        {
            Vector3 targetPosition = _playerTr.position + new Vector3(0, 1f, 0);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firePos.position, targetPosition); // firePos에서 targetPosition까지 선 그리기
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }
    }
}
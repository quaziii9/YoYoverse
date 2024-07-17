using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyFire : MonoBehaviour
{
    private Animator animator;
    private Transform playerTr;
    private Transform firePos;
    private EnemyAI enemyAI;
    private Transform enemyTr;

    private readonly int hashFire = Animator.StringToHash("Fire");

    public bool isFireAnimIng = false;
    private bool pendingIdleState = false;

    [Header("Bullet")]
    public GameObject bullet;

    public float detectionRange = 10.0f;

    public bool InPlayer;


    void Start()
    {
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();

        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = detectionRange;

        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public IEnumerator FireAfterRotation()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기 후 발사
        Fire();
    }

    public void Fire()
    {
        if (!isFireAnimIng)
        {
            isFireAnimIng = true;
            animator.SetTrigger(hashFire);
            FireBullet();
        }
    }


    public void FireBullet()
    {
        Vector3 direction = (playerTr.position - firePos.position).normalized;

        GameObject bulletInstance = ObjectPool.Instance.DequeueObject(bullet);
        bulletInstance.transform.position = firePos.position;
        bulletInstance.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * 100;
        }
    }

    public void OnFireAnimationEnd()
    {
        isFireAnimIng = false;

        if (pendingIdleState)
        {
            enemyAI.ChangeState(EnemyState.Idle);
            pendingIdleState = false;
        }
        else if (enemyAI.EnemyCurstate == EnemyState.ATTACK && InPlayer == true)
        {
            enemyAI.StartCoroutine(WaitAndFire());
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
            pendingIdleState = true;
        }
        else
        {
            if(enemyAI.EnemyCurstate == EnemyState.ATTACK)
            enemyAI.ChangeState(EnemyState.Idle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopFiring();
            InPlayer = false;
        }
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (firePos != null && playerTr != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firePos.position, playerTr.position);
            Gizmos.DrawSphere(playerTr.position, 0.2f);
        }
    }
}



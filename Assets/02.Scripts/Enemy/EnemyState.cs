using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IState
{
    void Enter();
    void ExecuteOnUpdate();
    void Exit();
}

public class IdleState : IState
{
    private EnemyAI enemyAI;
    private Coroutine rotateCoroutine;

    public IdleState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public void Enter()
    {
        enemyAI.animator.SetBool(enemyAI.Idle, true);
        rotateCoroutine = enemyAI.StartCoroutine(RotateCoroutine());
    }

    public void ExecuteOnUpdate()
    {
        // Add any other update logic if needed
    }

    public void Exit()
    {
        enemyAI.animator.SetBool(enemyAI.Idle, false);
        if (rotateCoroutine != null)
        {
            enemyAI.StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
    }

    private IEnumerator RotateCoroutine()
    {
        while (true)
        {
            float targetAngle = enemyAI.transform.eulerAngles.y + enemyAI.RotationAngle;
            yield return RotateToAngle(targetAngle);

            yield return new WaitForSeconds(1f); // Wait for 1 second before rotating back

            targetAngle = enemyAI.transform.eulerAngles.y - enemyAI.RotationAngle;
            yield return RotateToAngle(targetAngle);

            yield return new WaitForSeconds(1f); // Wait for 1 second before rotating again
        }
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        float currentAngle = enemyAI.transform.eulerAngles.y;
        float startAngle = currentAngle;
        float t = 0f;
        float duration = 2f; // 2 second duration for rotation

        while (t < duration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(startAngle, targetAngle, t / duration);
            enemyAI.transform.eulerAngles = new Vector3(enemyAI.transform.eulerAngles.x, angle, enemyAI.transform.eulerAngles.z);
            yield return null;
        }

        enemyAI.transform.eulerAngles = new Vector3(enemyAI.transform.eulerAngles.x, targetAngle, enemyAI.transform.eulerAngles.z);
    }
}

public class PatrolState : IState
{
    private EnemyAI enemyAI;

    public PatrolState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public void Enter()
    {
        enemyAI.enemyMoveAgent.patrolling = true;
        enemyAI.animator.SetBool(enemyAI.hashMove, true);
    }

    public void ExecuteOnUpdate()
    {
        // 상태 업데이트 코드
    }

    public void Exit()
    {
        enemyAI.enemyMoveAgent.patrolling = false;
        enemyAI.animator.SetBool(enemyAI.hashMove, false);
    }
}

public class TraceState : IState
{
    private EnemyAI enemyAI;

    public TraceState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public void Enter()
    {
        enemyAI.animator.SetBool(enemyAI.hashMove, true);
    }

    public void ExecuteOnUpdate()
    {
        enemyAI.enemyMoveAgent.SetDestination(enemyAI.playerTr.position);

        if (Vector3.Distance(enemyAI.playerTr.position, enemyAI.enemyTr.position) <= enemyAI.attackDist)
        {
            // 플레이어와의 사이에 장애물이 없으면 공격 상태로 전환
            if (!Physics.Linecast(enemyAI.firePos.position, enemyAI.playerTr.position, out RaycastHit hit))
            {
                enemyAI.ChangeState(EnemyState.ATTACK);
            }
        }
    }

    public void Exit()
    {
        enemyAI.animator.SetBool(enemyAI.hashMove, false);
    }
}


public class AttackState : IState
{
    private EnemyAI enemyAI;
    private Transform _enemyTr;
    private Transform _playerTr;
    private EnemyFire _enemyFire;
    private float rotationSpeed = 30f; // 회전 속도
    private float shootCooldown = 1f; // 발사 간 대기 시간
    private float lastShootTime;
    private NavMeshAgent _navMeshAgent;
    private Coroutine smoothRotationCoroutine;

    public AttackState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        _enemyTr = enemyAI.transform;
        _playerTr = enemyAI.playerTr;
        _enemyFire = enemyAI.enemyFire;
        _navMeshAgent = enemyAI.enemyMoveAgent.agent;
    }

    public void Enter()
    {
        // 부드러운 초기 회전 시작
        smoothRotationCoroutine = enemyAI.StartCoroutine(SmoothRotation());
    }

    public void ExecuteOnUpdate()
    {
        // 부드러운 회전이 완료된 후에만 발사 시도
        if (smoothRotationCoroutine == null)
        {
            Vector3 direction = (_playerTr.position - _enemyTr.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            _enemyTr.rotation = Quaternion.Slerp(_enemyTr.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            // 쿨다운이 지나고 발사 애니메이션이 끝났다면 발사
            if (Time.time >= lastShootTime + shootCooldown && _enemyFire.isFireAnimIng == false)
            {
                _enemyFire.StartCoroutine(_enemyFire.FireAfterRotation());
                lastShootTime = Time.time;
            }
        }
    }

    public void Exit()
    {
        if (smoothRotationCoroutine != null)
        {
            enemyAI.StopCoroutine(smoothRotationCoroutine);
        }
    }

    private IEnumerator SmoothRotation()
    {
        Vector3 direction = (_playerTr.position - _enemyTr.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (Quaternion.Angle(_enemyTr.rotation, targetRotation) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            _enemyTr.rotation = Quaternion.RotateTowards(_enemyTr.rotation, targetRotation, step);
            yield return null;
        }

        _enemyTr.rotation = targetRotation;
        smoothRotationCoroutine = null; // 회전 완료 후 코루틴을 null로 설정
        _enemyFire.StartCoroutine(_enemyFire.FireAfterRotation());
    }
}



public class DieState : IState
{
    private EnemyAI enemyAI;

    public DieState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    [System.Obsolete]
    public void Enter()
    {
        enemyAI.isDie = true;
        enemyAI.enemyMoveAgent.agent.Stop();

        enemyAI.animator.SetBool(enemyAI.hashDie, true);
    }

    public void ExecuteOnUpdate()
    {
        // 사망 상태 업데이트 코드
    }

    public void Exit()
    {
        // 사망 상태 종료 코드
    }
}

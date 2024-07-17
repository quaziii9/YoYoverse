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
    private Coroutine returnCoroutine;
    private Coroutine rotateCoroutine;
    private bool isReturning = false;

    public IdleState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public void Enter()
    {
        enemyAI.animator.SetBool(enemyAI.Idle, true);
        isReturning = true;
        returnCoroutine = enemyAI.StartCoroutine(ReturnToInitialTransform());
    }

    public void ExecuteOnUpdate()
    {
        // 필요한 경우 추가 로직
    }

    public void Exit()
    {
        enemyAI.animator.SetBool(enemyAI.Idle, false);
        if (returnCoroutine != null)
        {
            enemyAI.StopCoroutine(returnCoroutine);
        }
        if (rotateCoroutine != null)
        {
            enemyAI.StopCoroutine(rotateCoroutine);
        }
        isReturning = false;
    }

    private IEnumerator ReturnToInitialTransform()
    {
        Vector3 startPosition = enemyAI.transform.position;
        Quaternion startRotation = enemyAI.transform.rotation;
        float elapsedTime = 0f;
        float returnDuration = 2f; // 부드러운 전환을 위한 시간 (조절 가능)

        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / returnDuration;

            enemyAI.transform.position = Vector3.Lerp(startPosition, enemyAI.initialPosition, t);
            enemyAI.transform.rotation = Quaternion.Slerp(startRotation, enemyAI.initialRotation, t);

            yield return null;
        }

        enemyAI.transform.position = enemyAI.initialPosition;
        enemyAI.transform.rotation = enemyAI.initialRotation;

        isReturning = false;
        rotateCoroutine = enemyAI.StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine()
    {
        while (!isReturning)
        {
            float targetAngle = enemyAI.transform.eulerAngles.y + enemyAI.RotationAngle;
            yield return RotateToAngle(targetAngle);

            yield return new WaitForSeconds(1f);

            targetAngle = enemyAI.transform.eulerAngles.y - enemyAI.RotationAngle;
            yield return RotateToAngle(targetAngle);

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        float startAngle = enemyAI.transform.eulerAngles.y;
        float elapsedTime = 0f;
        float rotateDuration = 2f;

        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotateDuration;
            float angle = Mathf.LerpAngle(startAngle, targetAngle, t);

            enemyAI.transform.rotation = Quaternion.Euler(enemyAI.transform.eulerAngles.x, angle, enemyAI.transform.eulerAngles.z);

            yield return null;
        }

        enemyAI.transform.rotation = Quaternion.Euler(enemyAI.transform.eulerAngles.x, targetAngle, enemyAI.transform.eulerAngles.z);
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
       // enemyAI.animator.SetBool(enemyAI.hashMove, true);
    }

    public void ExecuteOnUpdate()
    {
        // 상태 업데이트 코드
    }

    public void Exit()
    {
        enemyAI.enemyMoveAgent.patrolling = false;
        //enemyAI.animator.SetBool(enemyAI.hashMove, false);
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
       // enemyAI.animator.SetBool(enemyAI.hashMove, true);
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
        //enemyAI.animator.SetBool(enemyAI.hashMove, false);
    }
}


public class AttackState : IState
{
    private EnemyAI enemyAI;
    private Transform _enemyTr;
    private Transform _playerTr;
    private EnemyFire _enemyFire;
    private float rotationSpeed = 10; // 회전 속도
    private Coroutine smoothRotationCoroutine;
    private bool hasFiredInitialShot = false; // 초기 발사 여부를 추적하는 변수

    public AttackState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        _enemyTr = enemyAI.transform;
        _playerTr = enemyAI.playerTr;
        _enemyFire = enemyAI.enemyFire;
    }

    public void Enter()
    {
        // 부드러운 초기 회전 시작 및 지속적인 회전 코루틴 실행
        smoothRotationCoroutine = enemyAI.StartCoroutine(SmoothRotation());
        _enemyFire.InPlayer = true;
    }

    public void ExecuteOnUpdate()
    {
        // 부드러운 회전을 지속적으로 수행
        if (smoothRotationCoroutine == null)
        {
            smoothRotationCoroutine = enemyAI.StartCoroutine(SmoothRotation());
        }
    }

    public void Exit()
    {
        if (smoothRotationCoroutine != null)
        {
            enemyAI.StopCoroutine(smoothRotationCoroutine);
            smoothRotationCoroutine = null;
        }
    }

    private IEnumerator SmoothRotation()
    {
        while (_enemyFire.InPlayer == true)
        {
            Vector3 direction = (_playerTr.position - _enemyTr.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)) * Quaternion.Euler(0, enemyAI.modelRotationOffset, 0);

            _enemyTr.rotation = Quaternion.Slerp(_enemyTr.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 초기 발사
            if (!hasFiredInitialShot && Quaternion.Angle(_enemyTr.rotation, targetRotation) < 5f)
            {
                _enemyFire.StartCoroutine(_enemyFire.FireAfterRotation());
                hasFiredInitialShot = true;
            }

            yield return null;
        }
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

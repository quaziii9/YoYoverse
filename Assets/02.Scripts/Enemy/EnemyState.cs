using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.AI;

public interface IState
{
    void Enter();
    void ExecuteOnUpdate();
    void Exit();
}

public class EnemyIdleState : IState
{
    private EnemyAI enemyAI;
    private Coroutine returnCoroutine;
    private Coroutine rotateCoroutine;
    private bool isReturning = false;

    public EnemyIdleState(EnemyAI enemyAI)
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
        // �ʿ��� ��� �߰� ����
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
        float returnDuration = 2f; // �ε巯�� ��ȯ�� ���� �ð� (���� ����)

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
            float targetAngle = enemyAI.transform.eulerAngles.y + enemyAI.rotationAngle;
            yield return RotateToAngle(targetAngle);

            yield return new WaitForSeconds(1f);

            targetAngle = enemyAI.transform.eulerAngles.y - enemyAI.rotationAngle;
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

public class EnemyPatrolState : IState
{
    private EnemyAI enemyAI;

    public EnemyPatrolState(EnemyAI enemyAI)
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
        // ���� ������Ʈ �ڵ�
    }

    public void Exit()
    {
        enemyAI.enemyMoveAgent.patrolling = false;
        //enemyAI.animator.SetBool(enemyAI.hashMove, false);
    }
}

public class EnemyTraceState : IState
{
    private EnemyAI enemyAI;

    public EnemyTraceState(EnemyAI enemyAI)
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
            // �÷��̾���� ���̿� ��ֹ��� ������ ���� ���·� ��ȯ
            if (!Physics.Linecast(enemyAI.firePos.position, enemyAI.playerTr.position, out RaycastHit hit))
            {
                enemyAI.ChangeState(EnemyState.Attack);
            }
        }
    }

    public void Exit()
    {
        //enemyAI.animator.SetBool(enemyAI.hashMove, false);
    }
}


public class EnemyAttackState : IState
{
    private EnemyAI enemyAI;
    private Transform _enemyTr;
    private Transform _playerTr;
    private EnemyFire _enemyFire;
    private float rotationSpeed = 10; // ȸ�� �ӵ�
    private Coroutine smoothRotationCoroutine;
    private bool hasFiredInitialShot = false; // �ʱ� �߻� ���θ� �����ϴ� ����

    public EnemyAttackState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        _enemyTr = enemyAI.transform;
        _playerTr = enemyAI.playerTr;
        _enemyFire = enemyAI.enemyFire;
    }

    public void Enter()
    {
        // �ε巯�� �ʱ� ȸ�� ���� �� �������� ȸ�� �ڷ�ƾ ����
        smoothRotationCoroutine = enemyAI.StartCoroutine(SmoothRotation());
        _enemyFire.InPlayer = true;
    }

    public void ExecuteOnUpdate()
    {
        // �ε巯�� ȸ���� ���������� ����
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

            // �ʱ� �߻�
            if (!hasFiredInitialShot && Quaternion.Angle(_enemyTr.rotation, targetRotation) < 5f)
            {
                _enemyFire.StartCoroutine(_enemyFire.FireAfterRotation());
                hasFiredInitialShot = true;
            }

            yield return null;
        }
    }
}

public class EnemyDieState : IState
{
    private EnemyAI enemyAI;

    public EnemyDieState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    [System.Obsolete]
    public void Enter()
    {
        enemyAI.isDie = true;
        enemyAI.enemyMoveAgent.agent.Stop();

        enemyAI.animator.SetBool(enemyAI.HashDie, true);
    }

    public void ExecuteOnUpdate()
    {
        // ��� ���� ������Ʈ �ڵ�
    }

    public void Exit()
    {
        // ��� ���� ���� �ڵ�
    }
}

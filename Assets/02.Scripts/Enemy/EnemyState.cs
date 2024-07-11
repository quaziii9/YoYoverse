using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PatrolState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.enemyFire.isFire = false;
        enemy.moveAgent.patrolling = true;

        enemy.animator.SetBool(enemy.hashMove, true);
    }

    public void Execute(EnemyAI enemy)
    {
        float dist = Vector3.Distance(enemy.playerTr.position, enemy.enemyTr.position);

        if (enemy.playerHp <= 0)
        {
            enemy.SetState(new DieState());
        }
        else if (dist <= enemy.attackDist)
        {
            enemy.SetState(new AttackState());
        }
        else if (dist <= enemy.traceDis)
        {
            enemy.SetState(new TraceState());
        }
    }

    public void Exit(EnemyAI enemy)
    {
        enemy.moveAgent.patrolling = false;
    }
}

public class TraceState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.enemyFire.isFire = false;
        enemy.animator.SetBool(enemy.hashMove, true);
    }

    public void Execute(EnemyAI enemy)
    {
        float dist = Vector3.Distance(enemy.playerTr.position, enemy.enemyTr.position);

        if (enemy.playerHp <= 0)
        {
            enemy.SetState(new DieState());
        }
        else if (dist <= enemy.attackDist)
        {
            enemy.SetState(new AttackState());
        }
        else if (dist > enemy.traceDis)
        {
            enemy.SetState(new PatrolState());
        }

        enemy.moveAgent.traceTarget = enemy.playerTr.position;
    }

    public void Exit(EnemyAI enemy)
    {
        enemy.moveAgent.Stop();
    }
}

public class AttackState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.enemyFire.isFire = true;
        enemy.moveAgent.Stop();
        enemy.animator.SetBool(enemy.hashMove, false);
    }

    public void Execute(EnemyAI enemy)
    {
        float dist = Vector3.Distance(enemy.playerTr.position, enemy.enemyTr.position);

        if (enemy.playerHp <= 0)
        {
            enemy.SetState(new DieState());
        }
        else if (dist > enemy.attackDist)
        {
            enemy.SetState(new TraceState());
        }
    }

    public void Exit(EnemyAI enemy)
    {
        enemy.enemyFire.isFire = false;
    }
}

public class DieState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        enemy.isDie = true;
        enemy.enemyFire.isFire = false;
        enemy.moveAgent.Stop();
        enemy.animator.SetBool(enemy.hashDie, true);

        foreach (Collider collider in enemy.childColliders)
        {
            collider.enabled = false;
        }
    }

    public void Execute(EnemyAI enemy) { }

    public void Exit(EnemyAI enemy) { }
}

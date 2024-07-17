using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void StartPatrolling()
    {
        patrolling = true;
        // 여기에 순찰 로직을 추가합니다.
    }

    public void StopPatrolling()
    {
        patrolling = false;
        // 여기에 순찰 중지 로직을 추가합니다.
    }
}



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveAgent : MonoBehaviour
{
    // 순찰 지점들을 저장하기 위한 List 타입 변수
    public List<Transform> wayPoints;

    // 다음 순찰 지점의 배열의 Index
    public int nextIdx = 0;
    public int endIdx = 0; // 마지막지점 찍은 횟수

    private NavMeshAgent agent;
    private Transform enemyTr;

    private float damping = 1.0f;

    private float moveSpeed = 1.5f;

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }

        set
        {
            moveSpeed = value;
        }
    }

    private bool _patrolling;

    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                // agent.speed = patrollSpeed;
                agent.speed = moveSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    // 추적 대상의 위치를 저장하는 변수
    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            // agent.speed = traceSpeed;
            agent.speed = moveSpeed * 2;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;

        agent.destination = pos;
        agent.isStopped = false;
    }

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.updateRotation = false;
        agent.speed = moveSpeed;

        var group = GameObject.Find("WayPointGroup");

        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            // 첫번째 요소엔 부모의 transform이 들어가기 때문 -> waypointgroup의 transform이 들어감, point들만 남게 
            wayPoints.RemoveAt(0);

            nextIdx = Random.Range(0, wayPoints.Count);
        }

        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        //최단 거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        if (agent.isPathStale) return;

        if (nextIdx < wayPoints.Count)
        {
            //다음 목적지를 wayPoints 배열에서 추출한 위치로 다음 목적지를 지정
            agent.destination = wayPoints[nextIdx].position;
            agent.isStopped = false;
        }
    }

    void Update()
    {
        if (agent.isStopped == false)
        {
            //NavMeshAgent가 가야할 방향 벡터를 쿼터니언 타입의 각도로 변환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            // 보간 함수를 사용해 점진적 회전
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);

        }

        if (!_patrolling) return;

        // NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부 계산 
        // velocity를 이용해서 현재속도를 받아오고, 목적지에 가까워 졌다고 판단되면 다음 목적지의 인덱스를 참조 -> 다음 목적지로 이동
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            #region 순차적으로 이동 
            //if (nextIdx == wayPoints.Count) endIdx++;
            //if (nextIdx == 0) endIdx++;
            //// 다음 목적지 배열 첨자를 계산 
            //if(endIdx !=0 && endIdx %2 !=0 && nextIdx < wayPoints.Count) 
            //    nextIdx = ++nextIdx;
            //else if (endIdx != 0 && endIdx % 2 == 0 && nextIdx <= wayPoints.Count)
            //    nextIdx = --nextIdx;
            #endregion
            nextIdx = Random.Range(0, wayPoints.Count);

            // 다음 목적지로 이동 명령 수행
            MoveWayPoint();
        }
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}

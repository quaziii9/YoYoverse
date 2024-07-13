using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Transform player; // 플레이어 트랜스폼
    public float viewAngle = 45f; // 시야각
    public float viewDistance = 10f; // 시야 거리
    public LayerMask obstacleMask; // 장애물 레이어 마스크
    public LayerMask playerMask; // 플레이어 레이어 마스크
    private MeshCollider meshCollider;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        // 시야 거리 내 모든 콜라이더 감지
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, playerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // 플레이어가 시야각 안에 있는지 확인
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // 플레이어와 시야 사이에 장애물이 없는지 확인
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    Debug.Log("플레이어 감지됨");
                    // 플레이어 감지 시 추가 로직 작성
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewDistance);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

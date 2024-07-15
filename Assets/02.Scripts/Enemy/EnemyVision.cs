using UnityEngine;
using EventLibrary;
using EnumTypes;

public class EnemyVision : MonoBehaviour
{
    public float viewAngle = 60f;
    public float viewDistance = 5f;
    public float viewHeight = 15f;
    public LayerMask playerLayer;
    public int detectionResolution = 10;

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        for (float y = -viewHeight; y <= viewHeight; y += viewHeight / detectionResolution)
        {
            for (float angle = -viewAngle / 2; angle <= viewAngle / 2; angle += viewAngle / detectionResolution)
            {
                Vector3 dir = DirFromAngle(angle, false);
                Vector3 rayStart = transform.position + Vector3.up * y;

                if (Physics.Raycast(rayStart, dir, out RaycastHit hit, viewDistance, playerLayer))
                {
                    Debug.Log("플레이어가 감지되었습니다!");

                    EventManager<EnemyEvents>.TriggerEvent(EnemyEvents.ChangeEnemyStateAttack);
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // 원뿔의 윤곽선 그리기
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        // 아래쪽 윤곽선
        Gizmos.DrawLine(transform.position - Vector3.up * viewHeight,
                        transform.position - Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(transform.position - Vector3.up * viewHeight,
                        transform.position - Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 위쪽 윤곽선
        Gizmos.DrawLine(transform.position + Vector3.up * viewHeight,
                        transform.position + Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(transform.position + Vector3.up * viewHeight,
                        transform.position + Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 수직 선
        Gizmos.DrawLine(transform.position - Vector3.up * viewHeight,
                        transform.position + Vector3.up * viewHeight);
        Gizmos.DrawLine(transform.position - Vector3.up * viewHeight + viewAngleA * viewDistance,
                        transform.position + Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(transform.position - Vector3.up * viewHeight + viewAngleB * viewDistance,
                        transform.position + Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 원뿔의 내부 영역 그리기
        for (float y = -viewHeight; y <= viewHeight; y += viewHeight / detectionResolution)
        {
            for (float angle = -viewAngle / 2; angle <= viewAngle / 2; angle += viewAngle / detectionResolution)
            {
                Vector3 dir = DirFromAngle(angle, false);
                Vector3 pos = transform.position + Vector3.up * y;
                Gizmos.DrawLine(pos, pos + dir * viewDistance);
            }
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
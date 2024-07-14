using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float viewAngle = 60f;
    public float viewDistance = 5f;
    public float viewHeight = 15f;
    public float forwardOffset = 1f; // 기즈모 시작 위치의 Z 오프셋
    public LayerMask playerLayer;
    public int detectionResolution = 10;

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Vector3 detectionStart = transform.position + transform.forward * forwardOffset;

        for (float y = -viewHeight; y <= viewHeight; y += viewHeight / detectionResolution)
        {
            for (float angle = -viewAngle / 2; angle <= viewAngle / 2; angle += viewAngle / detectionResolution)
            {
                Vector3 dir = DirFromAngle(angle, false);
                Vector3 rayStart = detectionStart + Vector3.up * y;

                if (Physics.Raycast(rayStart, dir, out RaycastHit hit, viewDistance, playerLayer))
                {
                    Debug.Log("플레이어가 감지되었습니다!");
                    // 여기에 플레이어 감지 시 수행할 동작을 추가하세요.
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 detectionStart = transform.position + transform.forward * forwardOffset;
        Gizmos.color = Color.yellow;

        // 원뿔의 윤곽선 그리기
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        // 아래쪽 윤곽선
        Gizmos.DrawLine(detectionStart - Vector3.up * viewHeight,
                        detectionStart - Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(detectionStart - Vector3.up * viewHeight,
                        detectionStart - Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 위쪽 윤곽선
        Gizmos.DrawLine(detectionStart + Vector3.up * viewHeight,
                        detectionStart + Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(detectionStart + Vector3.up * viewHeight,
                        detectionStart + Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 수직 선
        Gizmos.DrawLine(detectionStart - Vector3.up * viewHeight,
                        detectionStart + Vector3.up * viewHeight);
        Gizmos.DrawLine(detectionStart - Vector3.up * viewHeight + viewAngleA * viewDistance,
                        detectionStart + Vector3.up * viewHeight + viewAngleA * viewDistance);
        Gizmos.DrawLine(detectionStart - Vector3.up * viewHeight + viewAngleB * viewDistance,
                        detectionStart + Vector3.up * viewHeight + viewAngleB * viewDistance);

        // 원뿔의 내부 영역 그리기
        for (float y = -viewHeight; y <= viewHeight; y += viewHeight / detectionResolution)
        {
            for (float angle = -viewAngle / 2; angle <= viewAngle / 2; angle += viewAngle / detectionResolution)
            {
                Vector3 dir = DirFromAngle(angle, false);
                Vector3 pos = detectionStart + Vector3.up * y;
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
using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    public Transform npcTransform; // NPC의 Transform
    public Transform playerTransform; // 플레이어의 Transform
    public float fovAngle = 30.0f; // 시야각 절반 (60도의 절반)
    public float detectionRange = 5.0f; // 감지 거리
    public float verticalRange = 2.0f; // 수직 감지 범위

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 npcPosition = npcTransform.position;
        Vector3 playerPosition = playerTransform.position;

        // 수평 거리 및 수직 거리 계산
        Vector3 directionToPlayer = playerPosition - npcPosition;
        float horizontalDistance = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).magnitude;
        float verticalDistance = Mathf.Abs(directionToPlayer.y);

        // 수직 범위 내에 있는지 확인
        if (verticalDistance > verticalRange)
        {
            Debug.Log("플레이어가 수직 범위 밖에 있습니다.");
            return;
        }

        // 시야각 내에 있는지 확인
        float angleToPlayer = Vector3.Angle(npcTransform.forward, directionToPlayer);
        if (angleToPlayer <= fovAngle && horizontalDistance <= detectionRange)
        {
            Debug.Log("플레이어가 NPC의 시야각 내에 있습니다.");
        }
        else
        {
            Debug.Log("플레이어가 NPC의 시야각 밖에 있습니다.");
        }
    }

    void OnDrawGizmos()
    {
        if (npcTransform == null) return;

        Gizmos.color = Color.blue;

        Vector3 forward = npcTransform.forward;
        Vector3 npcPosition = npcTransform.position;

        // 시야각의 원뿔을 그리기
        for (float y = -verticalRange; y <= verticalRange; y += 0.5f)
        {
            Vector3 heightOffset = Vector3.up * y;
            for (float angle = -fovAngle; angle <= fovAngle; angle += 1.0f)
            {
                Vector3 direction = Quaternion.Euler(0, angle, 0) * forward * detectionRange;
                Gizmos.DrawRay(npcPosition + heightOffset, direction);
            }
        }

        // 수직 범위를 상단과 하단에 표시
        Gizmos.color = Color.red;
        Gizmos.DrawLine(npcPosition + Vector3.up * verticalRange, npcPosition + forward * detectionRange + Vector3.up * verticalRange);
        Gizmos.DrawLine(npcPosition - Vector3.up * verticalRange, npcPosition + forward * detectionRange - Vector3.up * verticalRange);
    }
}

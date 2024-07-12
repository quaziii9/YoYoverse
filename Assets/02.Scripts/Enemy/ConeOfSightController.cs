using UnityEngine;

[ExecuteAlways]
public class ConeOfSightController : MonoBehaviour
{
    public Material coneOfSightMaterial; // 이 스크립트가 조절할 머티리얼
    public Color color = Color.white;
    public float viewAngle = 45f;
    public float angleStrength = 1f;
    public float viewIntervals = 0.0075f;
    public float viewIntervalsStep = 0.0025f;
    public float innerCircleSize = 0.05f;
    public float circleStrength = 70f;
    public float viewDistance = 10f;

    private void Update()
    {
        if (coneOfSightMaterial != null)
        {
            coneOfSightMaterial.SetColor("_Color", color);
            coneOfSightMaterial.SetFloat("_ViewAngle", viewAngle);
            coneOfSightMaterial.SetFloat("_AngleStrength", angleStrength);
            coneOfSightMaterial.SetFloat("_ViewIntervals", viewIntervals);
            coneOfSightMaterial.SetFloat("_ViewIntervalsStep", viewIntervalsStep);
            coneOfSightMaterial.SetFloat("_InnerCircleSize", innerCircleSize);
            coneOfSightMaterial.SetFloat("_CircleStrength", circleStrength);
            coneOfSightMaterial.SetFloat("_ViewDistance", viewDistance);
        }
    }

    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때마다 업데이트
        Update();
    }
}

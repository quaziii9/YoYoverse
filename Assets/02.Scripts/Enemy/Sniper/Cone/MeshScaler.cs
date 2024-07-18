using UnityEngine;

public class MeshScaler : MonoBehaviour
{
    public Vector3 scale = new Vector3(1, 1, 1);
    [Range(0.01f, 90f)]
    public float viewAngle = 45f;
    private MeshRenderer meshRenderer;
    
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int ViewAngle = Shader.PropertyToID("_ViewAngle");

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateProperties();
    }

    private void Update()
    {
      // UpdateProperties();
    }

    private void UpdateProperties()
    {
        if (meshRenderer != null && meshRenderer.sharedMaterial != null)
        {
            // 편집 모드에서는 Material을 직접 수정하지 않습니다.
            if (Application.isPlaying)
            {
                meshRenderer.sharedMaterial.SetVector(Scale, new Vector4(scale.x, scale.y, scale.z, 1));
                meshRenderer.sharedMaterial.SetFloat(ViewAngle, viewAngle);
            }

            // Transform 스케일 업데이트는 편집 모드에서도 안전합니다.
            meshRenderer.transform.localScale = scale;
        }
    }
}
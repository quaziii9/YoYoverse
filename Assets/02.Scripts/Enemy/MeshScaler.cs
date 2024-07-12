using UnityEngine;

[ExecuteAlways]
public class MeshScaler : MonoBehaviour
{
    public Vector3 scale = new Vector3(1, 1, 1);
    [Range(0.01f, 90f)]
    public float viewAngle = 45f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateProperties();
    }

    void Update()
    {
        // Update the properties in the shader and mesh renderer if the scale or angle values change
        UpdateProperties();
    }

    void UpdateProperties()
    {
        // Update the material's _Scale and _ViewAngle properties
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.SetVector("_Scale", new Vector4(scale.x, scale.y, scale.z, 1));
            meshRenderer.material.SetFloat("_ViewAngle", viewAngle);

            // Update the transform scale of the mesh renderer
            meshRenderer.transform.localScale = scale;
        }
    }
}

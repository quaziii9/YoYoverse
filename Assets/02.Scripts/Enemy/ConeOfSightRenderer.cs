using UnityEngine;

public class ConeOfSightRenderer : MonoBehaviour
{
    private static readonly int sViewDepthTexturedID = Shader.PropertyToID("_ViewDepthTexture");
    private static readonly int sViewSpaceMatrixID = Shader.PropertyToID("_ViewSpaceMatrix");
    private static readonly int sViewDistanceID = Shader.PropertyToID("_ViewDistance");
    private static readonly int sViewAngleID = Shader.PropertyToID("_ViewAngle");

    public Camera ViewCamera;

    [SerializeField]
    private float viewDistance = 10f;
    public float ViewDistance
    {
        get { return viewDistance; }
        set
        {
            viewDistance = value;
            UpdateViewParameters();
        }
    }

    [SerializeField]
    private float viewAngle = 60f;
    public float ViewAngle
    {
        get { return viewAngle; }
        set
        {
            viewAngle = value;
            UpdateViewParameters();
        }
    }

    [SerializeField]
    private Color viewColor = Color.white;
    public Color ViewColor
    {
        get { return viewColor; }
        set
        {
            viewColor = value;
            UpdateViewParameters();
        }
    }

    private Material mMaterial;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mMaterial = renderer.material;
        renderer.material = mMaterial;

        RenderTexture depthTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 24, RenderTextureFormat.Depth);
        ViewCamera.targetTexture = depthTexture;

        mMaterial.SetTexture(sViewDepthTexturedID, ViewCamera.targetTexture);

        UpdateViewParameters();
    }

    private void Update()
    {
        ViewCamera.Render();
        mMaterial.SetMatrix(sViewSpaceMatrixID, ViewCamera.projectionMatrix * ViewCamera.worldToCameraMatrix);
    }

    private void UpdateViewParameters()
    {
        ViewCamera.farClipPlane = viewDistance;
        ViewCamera.fieldOfView = viewAngle;
        transform.localScale = new Vector3(viewDistance * 2, transform.localScale.y, viewDistance * 2);
        mMaterial.SetFloat(sViewAngleID, viewAngle);
        mMaterial.SetColor("_Color", viewColor);
        mMaterial.SetFloat(sViewDistanceID, viewDistance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
        Gizmos.DrawWireSphere(Vector3.zero, viewDistance);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class EnemyVisionShadow : MonoBehaviour
{
    public float viewAngle = 60f;
    public float viewDistance = 5f;
    public int detectionResolution = 10;
    public LayerMask groundLayer;
    public Material visionMaterial; // URP용 Material을 할당

    private MeshFilter meshFilter;

    private void Start()
    {
        Debug.Log("Start 메서드 호출");
        meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (visionMaterial == null)
        {
            Debug.LogError("Material이 할당되지 않았습니다. 에디터에서 visionMaterial을 할당해주세요.");
            return;
        }
        meshRenderer.material = visionMaterial;
    }

    private void Update()
    {
        Debug.Log("Update 메서드 호출");
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[detectionResolution + 2];
        int[] triangles = new int[detectionResolution * 3];

        vertices[0] = transform.position;
        Debug.Log("Vertices[0]: " + vertices[0]);

        float angleStep = viewAngle / detectionResolution;
        for (int i = 0; i <= detectionResolution; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            Vector3 dir = DirFromAngle(angle, false);
            Vector3 vertex;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewDistance, groundLayer))
            {
                vertex = hit.point;
                Debug.Log("Raycast hit at: " + vertex);
            }
            else
            {
                vertex = transform.position + dir * viewDistance;
                Debug.Log("No hit, vertex set to: " + vertex);
            }

            vertices[i + 1] = vertex;

            if (i > 0)
            {
                triangles[(i - 1) * 3] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = i + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        Debug.Log("Mesh updated with vertices count: " + vertices.Length);
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

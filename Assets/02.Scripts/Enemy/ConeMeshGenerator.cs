using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeMeshGenerator : MonoBehaviour
{
    [Range(0.01f, 90f)]
    public float viewAngle = 45f;
    public float height = 15f;
    public int segments = 24;

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        UpdateConeMesh();
    }

    void Update()
    {
        UpdateConeMesh();
    }

    void UpdateConeMesh()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        meshFilter.mesh = GenerateConeMesh(viewAngle, height, segments);
    }

    Mesh GenerateConeMesh(float angle, float height, int segments)
    {
        Mesh mesh = new Mesh();

        float radius = height * Mathf.Tan(angle * Mathf.Deg2Rad / 2);
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;  // 중심점
        vertices[1] = new Vector3(0, height, 0);  // 정점

        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(currentAngle) * radius;
            float z = Mathf.Sin(currentAngle) * radius;

            vertices[i + 2] = new Vector3(x, 0, z);

            int nextIndex = (i + 1) % segments;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = nextIndex + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}

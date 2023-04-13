using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public Gradient gradient;


    [Range(1, 200)]
    public int widthX = 50;
    [Range(1, 200)]
    public int lengthZ = 50;



    public float amplitude1 = 49f;
    public float amplitude2 = 5.6f;
    public float amplitude3 = 1.68f;

    [Range(0.0f, 1.0f)]
    public float frequency1 = 0.02f;
    [Range(0.0f, 0.1f)]
    public float frequency2 = 0.1f;
    [Range(0.0f, 1.0f)]
    public float frequency3 = 0.6f;

    public int translateX, translateZ = 0;
    public bool plateau = false;

    private float minTerrainHeight = 0.0f;
    private float maxTerrainHeight = 0.0f;

    float GetNoiseSample(int x, int z)
    {
        float r =
                      amplitude1 * Mathf.PerlinNoise((x - translateX) * frequency1, (z - translateZ) * frequency1)
                    + amplitude2 * Mathf.PerlinNoise((x - translateX) * frequency2, (z - translateZ) * frequency2)
                    + amplitude3 * Mathf.PerlinNoise((x - translateX) * frequency3, (z - translateZ) * frequency3);
        float extraHeight = (amplitude1 + amplitude2 + amplitude3) / 3;
        r -= extraHeight;
        return (plateau ? ((int)(r / 5) * 5) : r);
    }

    void CreateShape()
    {
        vertices = new Vector3[(widthX + 1) * (lengthZ + 1)];

        for(int i = 0, z = 0; z <= lengthZ; z++)
        {
            for(int x = 0; x <= widthX; x++, i++)
            {
                float y = GetNoiseSample(x, z);
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight) maxTerrainHeight = y;
                if (y < minTerrainHeight) minTerrainHeight = y;
            }
        }

        // 6 vertices per square, x * z squares
        triangles = new int[widthX * lengthZ * 6];
        int vertex = 0;
        int tris = 0;

        for(int z = 0; z < lengthZ; z++)
        {
            for (int x = 0; x < widthX; x++)
            {
                triangles[tris + 0] = vertex + 0;
                triangles[tris + 1] = vertex + widthX + 1;
                triangles[tris + 2] = vertex + 1;
                triangles[tris + 3] = vertex + 1;
                triangles[tris + 4] = vertex + widthX + 1;
                triangles[tris + 5] = vertex + widthX + 2;

                vertex++;
                tris += 6;
            }
            vertex++;
        }

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= lengthZ; z++)
        {
            for (int x = 0; x <= widthX; x++, i++)
            {
                float height = vertices[i].y;
                float normalizedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, height);
                colors[i] = gradient.Evaluate(normalizedHeight);
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        CreateShape();
        UpdateMesh();
    }

}

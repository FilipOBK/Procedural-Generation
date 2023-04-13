using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SphereTerrain
{
    ShapeGenerator shapeGen;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 AxisA;
    Vector3 AxisB;

    public SphereTerrain(ShapeGenerator shapeGen, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGen = shapeGen;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        AxisA = new Vector3(localUp.y, localUp.z, localUp.x);
        AxisB = Vector3.Cross(localUp, AxisA);
    }

    public void ConstructMesh()
    {
        // Each face is a square of r * r vertices
        Vector3[] vertices = new Vector3[resolution * resolution];

        // x * x face makes grid of (x - 1)^2 sub-squares, each of which has 2 triangles, each of which made of 3 vertices
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];

        // Do this the number of times equal to the number of vertices
        for(int y = 0, triIndex = 0, vertIndex = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                // Add vertices on a face in the direction of LocalUp
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2.0f * AxisA + (percent.y - 0.5f) * 2.0f * AxisB;
                // Set all points at equal distance from center
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                // Modify
                vertices[vertIndex] = shapeGen.CalculatePointOnPlanet(pointOnUnitSphere);

                if(x != resolution - 1 && y != resolution - 1)
                {
                    // calculate triangles on the face mesh
                    triangles[triIndex + 0] = vertIndex;
                    triangles[triIndex + 1] = vertIndex + resolution + 1;
                    triangles[triIndex + 2] = vertIndex + resolution;

                    triangles[triIndex + 3] = vertIndex;
                    triangles[triIndex + 4] = vertIndex + 1;
                    triangles[triIndex + 5] = vertIndex + resolution + 1;

                    triIndex += 6;
                }

                vertIndex++;
            }
        }

        // redo mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LakeGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int xSize = 200;
    public int zSize = 200;

    public float verticesSize = .5f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
    }

    private void Update()
    {
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // Places the vertices using Perlin noise for the Y axis
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x * verticesSize, 0, z * verticesSize);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        // Creates every Quad
        for (int z = 0; z < zSize; ++z)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[0 + tris] = vert + 0;
                triangles[1 + tris] = vert + xSize + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + xSize + 1;
                triangles[5 + tris] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        uvs = new Vector2[vertices.Length];
        // Calculates the colors for texturing
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
    }
}

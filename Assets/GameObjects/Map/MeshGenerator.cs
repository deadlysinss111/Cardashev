using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public float verticesSize = .5f;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    float offSetX;
    float offSetY;

    public float noiseDensity = 0.05f;
    public float noiseHeight = 1f;

    // Start is called before the first frame update
    void Start()
    {
        offSetX = Random.Range(0f, 9999f);
        offSetY = Random.Range(0f, 9999f);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize+1)*(zSize+1)];

        // Places the vertices using Perlin noise for the Y axis
        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetPerlinNoise(x, z);
                vertices[i] = new Vector3(x * verticesSize, y, z * verticesSize);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

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

        colors = new Color[vertices.Length];
        // Calculates the colors for texturing
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);

                i++;
            }
        }
    }

    float GetPerlinNoise(float x, float y)
    {
        float xCoord = (float)x * noiseDensity + offSetX;
        float yCoord = (float)y * noiseDensity + offSetY;

        return Mathf.PerlinNoise(xCoord, yCoord) * noiseHeight;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}

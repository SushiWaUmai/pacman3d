using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static Mesh MeshFromHeightMap(bool[,] heightMap, int tileSize)
    {
        Mesh result = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int i = 0;

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++, i++)
            {
                int y = heightMap[x, z] ? 1 : 0;
                verts.Add(new Vector3(x, y, z) * tileSize);
                verts.Add(new Vector3(x + 1, y, z) * tileSize);
                verts.Add(new Vector3(x, y, z + 1) * tileSize);
                verts.Add(new Vector3(x + 1, y, z + 1) * tileSize);

                tris.Add(0 + i * 4);
                tris.Add(2 + i * 4);
                tris.Add(1 + i * 4);
                tris.Add(2 + i * 4);
                tris.Add(3 + i * 4);
                tris.Add(1 + i * 4);
            }
        }

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++)
            {
                if(x + 1 < heightMap.GetLength(0))
                {
                    if (heightMap[x, z] ^ heightMap[x + 1, z])
                    {

                        verts.Add(new Vector3(x + 1, 0, z + 1) * tileSize);
                        verts.Add(new Vector3(x + 1, 0, z) * tileSize);
                        verts.Add(new Vector3(x + 1, 1, z + 1) * tileSize);
                        verts.Add(new Vector3(x + 1, 1, z) * tileSize);


                        if (heightMap[x, z])
                        {
                            tris.Add(1 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(0 + i * 4);
                            tris.Add(1 + i * 4);
                            tris.Add(3 + i * 4);
                            tris.Add(2 + i * 4);
                        }
                        else
                        {
                            tris.Add(0 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(1 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(3 + i * 4);
                            tris.Add(1 + i * 4);
                        }
                        i++;

                    }
                }

                if(z + 1 < heightMap.GetLength(1))
                {
                    if (heightMap[x, z] ^ heightMap[x, z + 1])
                    {
                        verts.Add(new Vector3(x, 0, z + 1) * tileSize);
                        verts.Add(new Vector3(x + 1, 0, z + 1) * tileSize);
                        verts.Add(new Vector3(x, 1, z + 1) * tileSize);
                        verts.Add(new Vector3(x + 1, 1, z + 1) * tileSize);

                        if (heightMap[x, z])
                        {
                            tris.Add(1 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(0 + i * 4);
                            tris.Add(1 + i * 4);
                            tris.Add(3 + i * 4);
                            tris.Add(2 + i * 4);
                        }
                        else
                        {
                            tris.Add(0 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(1 + i * 4);
                            tris.Add(2 + i * 4);
                            tris.Add(3 + i * 4);
                            tris.Add(1 + i * 4);
                        }
                        i++;
                    }
                }
            }
        }

        result.vertices = verts.ToArray();
        result.triangles = tris.ToArray();
        result.RecalculateNormals();

        return result;
    }

    public static Mesh MinimapMeshFromHeighMap(int width, int height, int tileSize)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0) * tileSize,
            new Vector3(0, 0, height) * tileSize,
            new Vector3(width, 0, height) * tileSize
        };

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        result.vertices = verts;
        result.triangles = tris;
        result.uv = uv;
        result.RecalculateNormals();

        return result;
    }
}
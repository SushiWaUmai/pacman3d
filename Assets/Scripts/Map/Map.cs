using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class Map : ScriptableObject
{
    [SerializeField, ShowAssetPreview] private Texture2D mapTexture;
    private bool[,] walls;
    private List<Vector2Int> palletPostions = new List<Vector2Int>();
    private List<Vector2Int> powerPalletPositions = new List<Vector2Int>();

    private const int tileSize = 2;

    [Button]
    public void LoadMap()
    {
        LoadMapTexture();
        CreateMap();
    }

    private void LoadMapTexture()
    {
        walls = new bool[mapTexture.width, mapTexture.height];

        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private void GenerateTile(int x, int y)
    {
        Color col = mapTexture.GetPixel(x, y);

        if (col.a == 0)
            return;
        if (col == Color.black)
            walls[x, y] = true;
        else if(col == Color.yellow)
            palletPostions.Add(new Vector2Int(x, y));
        else if (col == Color.red)
            powerPalletPositions.Add(new Vector2Int(x, y));
    }

    private void CreateMap()
    {
        GameObject map = new GameObject(mapTexture.name);
        GameObject wallGO = new GameObject("Walls");
        wallGO.transform.parent = map.transform;
        wallGO.AddComponent<MeshRenderer>();
        MeshFilter wallMesh = wallGO.AddComponent<MeshFilter>();
        wallMesh.mesh = MeshGenerator.MeshFromHeightMap(walls, tileSize);
        
        //GameObject collectables = new GameObject("Collectables");
        //GameObject pallets = new GameObject("Pallets");
        //pallets.transform.parent = collectables.transform;

        //GameObject powerPallets = new GameObject("Power Pallets");
        //powerPallets.transform.parent = collectables.transform;
    }
}
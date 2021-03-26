using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class Map : ScriptableObject
{
    [Header("Map Properties")]
    [SerializeField, ShowAssetPreview] private Texture2D mapTexture;
    [SerializeField] private int tileSize = 2;
    [SerializeField] private Material mapMaterial;
    
    [Header("Minimap")]
    [ShowNonSerializedField, ShowAssetPreview] private Texture2D minimapTexture;
    [SerializeField] private LayerMask minimapLayer;
    [SerializeField] private Material miniMapMaterial;

    [Header("Prefabs")]
    [SerializeField] private GameObject pellet;
    [SerializeField] private GameObject powerPellet;
    [SerializeField] private GameObject portal;
    private bool[,] walls;
    private List<Vector2Int> pelletPositions = new List<Vector2Int>();
    private List<Vector2Int> powerPelletPositions = new List<Vector2Int>();

    

    [Button]
    public void LoadMap()
    {
        LoadMapTexture();
        CreateMap();
    }

    private void LoadMapTexture()
    {
        walls = new bool[mapTexture.width, mapTexture.height];
        pelletPositions = new List<Vector2Int>();
        powerPelletPositions = new List<Vector2Int>();

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
        else if (col == new Color(1, 1, 0, 1))
            pelletPositions.Add(new Vector2Int(x, y));
        else if (col == Color.red)
            powerPelletPositions.Add(new Vector2Int(x, y));
    }

    private void CreateMap()
    {
        GameObject map = new GameObject(mapTexture.name);
        map.AddComponent<GridbasedPathfinding>().CreateGrid(walls);
        CreateWalls(map.transform);
        CreateCollectables(map.transform);
        CreatePortals(map.transform);
        CreateMiniMap(map.transform);
    }

    private void CreateWalls(Transform parentTransform)
    {
        GameObject wallGO = new GameObject("Walls");
        wallGO.transform.parent = parentTransform;
        MeshRenderer wallRenderer = wallGO.AddComponent<MeshRenderer>();
        MeshFilter wallMeshFilter = wallGO.AddComponent<MeshFilter>();
        MeshCollider wallCollider = wallGO.AddComponent<MeshCollider>();
        Mesh wallMesh = MeshGenerator.MeshFromHeightMap(walls, tileSize); ;
        wallMeshFilter.sharedMesh = wallMesh;
        wallCollider.sharedMesh = wallMesh;
        wallRenderer.sharedMaterial = mapMaterial;
    }

    private void CreateCollectables(Transform parentTransform)
    {
        GameObject collectables = new GameObject("Collectables");
        collectables.transform.parent = parentTransform;

        CreatePellets(collectables.transform);
        CreatePowerPellets(collectables.transform);
    }

    private void CreatePellets(Transform parentTransform)
    {
        GameObject pelletHolder = new GameObject("Pallets");
        pelletHolder.transform.parent = parentTransform;
        CreatePrefabs(pelletHolder.transform, pellet, pelletPositions, "Pellet");
    }

    private void CreatePowerPellets(Transform parentTransform)
    {
        GameObject powerPelletHolder = new GameObject("Power Pallets");
        powerPelletHolder.transform.parent = parentTransform;
        CreatePrefabs(powerPelletHolder.transform, powerPellet, powerPelletPositions, "Power Pellet");
    }

    private void CreatePortals(Transform parentTransform)
    {
        GameObject portalHolder = new GameObject("Portals");
        portalHolder.transform.parent = parentTransform;

        int xLength = walls.GetLength(0);
        int zLength = walls.GetLength(1);
        int i = 0;

        for (int x = 0; x < xLength; x++)
        {
            if(!walls[x, 0] && !walls[x, zLength - 1])
            {
                Portal p1 = Instantiate(portal, new Vector3(x, 0, 0) * tileSize + new Vector3(1, 1, 0) * tileSize / 2, Quaternion.identity, portalHolder.transform).GetComponent<Portal>();
                Portal p2 = Instantiate(portal, new Vector3(x, 0, zLength) * tileSize + new Vector3(1, 1, 0) * tileSize / 2, Quaternion.identity, portalHolder.transform).GetComponent<Portal>();

                p1.linkedPortal = p2;
                p2.linkedPortal = p1;

                p1.gameObject.name = $"PortalA {i}";
                p2.gameObject.name = $"PortalB {i}";
                i++;
            }
        }
        for (int z = 0; z < zLength; z++)
        {
            if(!walls[0, z] && !walls[xLength - 1, z])
            {
                // Needs to be rotated by 90 degrees
                Portal p1 = Instantiate(portal, new Vector3(0, 0, z) * tileSize + new Vector3(0, 1, 1) * tileSize / 2, Quaternion.AngleAxis(90, Vector3.up), portalHolder.transform).GetComponent<Portal>();
                Portal p2 = Instantiate(portal, new Vector3(xLength, 0, z) * tileSize + new Vector3(0, 1, 1) * tileSize / 2, Quaternion.AngleAxis(90, Vector3.up), portalHolder.transform).GetComponent<Portal>();

                p1.linkedPortal = p2;
                p2.linkedPortal = p1;

                p1.gameObject.name = $"PortalA {i}";
                p2.gameObject.name = $"PortalB {i}";
                i++;
            }
        }
    }

    private void CreatePrefabs(Transform prefabHolder, GameObject prefab, List<Vector2Int> pos, string name)
    {
        for (int i = 0; i < pos.Count; i++)
        {
            GameObject pelletGO = Instantiate(prefab, new Vector3(pos[i].x, 0, pos[i].y) * tileSize + Vector3.one * tileSize / 2f - Vector3.up * tileSize / 4f, Quaternion.identity, prefabHolder);
            pelletGO.name = $"{name} {i}";
        }
    }

    private void CreateMiniMap(Transform parentTransform)
    {
        GameObject miniMapHolder = new GameObject("Mini Map");
        miniMapHolder.layer = minimapLayer.ToLayer();
        miniMapHolder.transform.parent = parentTransform;

        GameObject minimapWalls = new GameObject("Walls");
        minimapWalls.layer = minimapLayer.ToLayer();
        minimapWalls.transform.position += Vector3.down * tileSize;
        minimapWalls.transform.parent = miniMapHolder.transform;
        MeshRenderer wallMeshRenderer = minimapWalls.AddComponent<MeshRenderer>();
        MeshFilter wallMeshFilter = minimapWalls.AddComponent<MeshFilter>();
        wallMeshRenderer.sharedMaterial = miniMapMaterial;
        wallMeshFilter.sharedMesh = MeshGenerator.MinimapMeshFromHeighMap(walls.GetLength(0), walls.GetLength(1), tileSize);
        minimapTexture = CreateMiniMapWallTexture();
        wallMeshRenderer.sharedMaterial.SetTexture("_MainTex", minimapTexture);
    }

    private Texture2D CreateMiniMapWallTexture()
    {
        Texture2D result = new Texture2D(mapTexture.width, mapTexture.height);
        result.filterMode = FilterMode.Point;

        for (int x = 0; x < result.width; x++)
        {
            for (int y = 0; y < result.height; y++)
            {
                if (mapTexture.GetPixel(x, y) == Color.black)
                    result.SetPixel(x, y, Color.black);
                else
                    result.SetPixel(x, y, Color.gray);
            }
        }

        result.Apply();
        return result;
    }
}
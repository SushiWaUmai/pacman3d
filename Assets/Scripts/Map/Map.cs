using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class Map : ScriptableObject
{
    private static Map current;

    [Header("Map Properties")]
    [SerializeField, ShowAssetPreview] private Texture2D mapTexture;
    [SerializeField] private int tileSize = 2;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material ghostTraversableWallMaterial;
    
    [Header("Minimap")]
    [ShowNonSerializedField, ShowAssetPreview] private Texture2D minimapTexture;
    [SerializeField] private LayerMask minimapLayer;
    [SerializeField] private Material miniMapMaterial;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject pellet;
    [SerializeField] private GameObject powerPellet;
    [SerializeField] private GameObject portal;

    [Header("Tile Colors")]
    [SerializeField] private Color wallColor;
    [SerializeField] private Color ghostWallColor;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color pelletColor;
    [SerializeField] private Color powerPelletColor;

    [Header("Ghost Exit")]
    [SerializeField] private LayerMask ghostTraversableLayer;
    private bool[,] walls;
    private List<Vector2Int> pelletPositions = new List<Vector2Int>();
    private List<Vector2Int> powerPelletPositions = new List<Vector2Int>();
    private List<Vector2Int> ghostTraversablePositions = new List<Vector2Int>();
    private Vector2Int playerPosition;
    

    [Button]
    public void LoadMap()
    {
        current = this;
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
        if (col == wallColor)
            walls[x, y] = true;
        else if (col == pelletColor)
            pelletPositions.Add(new Vector2Int(x, y));
        else if (col == powerPelletColor)
            powerPelletPositions.Add(new Vector2Int(x, y));
        else if (col == ghostWallColor)
            ghostTraversablePositions.Add(new Vector2Int(x, y));
        else if (col == playerColor)
            playerPosition = new Vector2Int(x, y);
    }

    private void CreateMap()
    {
        GameObject map = new GameObject(mapTexture.name);
        map.AddComponent<GridbasedPathfinding>().CreateGrid(walls);
        CreateWalls(map.transform);
        CreateCollectables(map.transform);
        CreatePortals(map.transform);
        CreateMiniMap(map.transform);

        // Create Player
        CreatePrefab(map.transform, playerPrefab, playerPosition, "Pacman Player");
    }

    private void CreateWalls(Transform parentTransform)
    {
        // Add a GameObject to hold the walls
        GameObject wallHolder = new GameObject("Walls");
        wallHolder.transform.parent = parentTransform;

        // Create the walls
        CreateNormalWalls(wallHolder.transform);
        CreateGhostTraversableWalls(wallHolder.transform);
    }

    private void CreateGhostTraversableWalls(Transform parentTransform)
    {
        // Create GameObject and set parent and layer
        GameObject ghostTraversable = new GameObject("Ghost Traversable Walls");
        ghostTraversable.transform.parent = parentTransform;
        ghostTraversable.layer = ghostTraversableLayer.ToLayer();

        // Add Components to GameObject
        MeshRenderer wallRenderer = ghostTraversable.AddComponent<MeshRenderer>();
        MeshFilter wallMeshFilter = ghostTraversable.AddComponent<MeshFilter>();
        MeshCollider wallCollider = ghostTraversable.AddComponent<MeshCollider>();

        // Create height map from list of vec2ints
        bool[,] ghostExitHeightMap = new bool[mapTexture.width, mapTexture.height];
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                if (ghostTraversablePositions.Contains(new Vector2Int(x, y)))
                    ghostExitHeightMap[x, y] = true;
            }
        }

        // Create a mesh from heightmap
        Mesh wallMesh = MeshGenerator.MeshCubesFromHeightMap(ghostExitHeightMap, tileSize);

        // Make mesh convex
        wallCollider.convex = true;

        // Add Mesh to components
        wallCollider.sharedMesh = wallMesh;
        wallMeshFilter.sharedMesh = wallMesh;
        wallRenderer.sharedMaterial = ghostTraversableWallMaterial;
    }

    private void CreateNormalWalls(Transform parentTransform)
    {
        // Create GameObject and set parent
        GameObject wallGO = new GameObject("Normal Wall");
        wallGO.transform.parent = parentTransform;

        // Add Components to GameObjects
        MeshRenderer wallRenderer = wallGO.AddComponent<MeshRenderer>();
        MeshFilter wallMeshFilter = wallGO.AddComponent<MeshFilter>();
        MeshCollider wallCollider = wallGO.AddComponent<MeshCollider>();
        
        // Create Mesh
        Mesh wallMesh = MeshGenerator.MeshFromHeightMap(walls, tileSize);

        // Add Mesh to Components
        wallMeshFilter.sharedMesh = wallMesh;
        wallCollider.sharedMesh = wallMesh;
        wallRenderer.sharedMaterial = wallMaterial;
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
            CreatePrefab(prefabHolder, prefab, pos[i], $"{name} {i}");
        }
    }

    private void CreatePrefab(Transform prefabHolder, GameObject prefab, Vector2Int pos, string name)
    {
        GameObject pelletGO = Instantiate(prefab, IndexToPosition(pos) - Vector3.up * tileSize / 4f, Quaternion.identity, prefabHolder);
        pelletGO.name = name;
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

    public static Vector2Int PositionToIndex(Vector3 position)
    {
        return Vector2Int.FloorToInt(new Vector2(position.x, position.z) / current.tileSize);
    }

    public static Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(index.x, 0, index.y) * current.tileSize + Vector3.one * current.tileSize / 2f;
    }

    public static Vector3 AlignToGrid(Vector3 position)
    {
        return Vector3Int.FloorToInt(position) + Vector3.one * current.tileSize / 2f;
    }
}
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
    [SerializeField] private GameObject intersection;

    [Header("Ghosts")]
    [SerializeField] private GameObject redGhost;
    [SerializeField] private GameObject cyanGhost;
    [SerializeField] private GameObject pinkGhost;
    [SerializeField] private GameObject orangeGhost;

    [Header("Tile Colors")]
    [SerializeField] private Color wallColor;
    [SerializeField] private Color ghostWallColor;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color pelletColor;
    [SerializeField] private Color powerPelletColor;

    [Header("Ghost Colors")]
    [SerializeField, Tooltip("Color of Blinky")] private Color redGhostColor;
    [SerializeField, Tooltip("Color of Inky")] private Color cyanGhostColor;
    [SerializeField, Tooltip("Color of Pinky")] private Color pinkGhostColor;
    [SerializeField, Tooltip("Color of Clyde")] private Color orangeGhostColor;

    [Header("Ghost Exit")]
    [SerializeField] private LayerMask ghostTraversableLayer;

    // Wall Positions
    private bool[,] walls;
    private List<Vector2Int> ghostTraversablePositions = new List<Vector2Int>();

    // Pellet positions
    private List<Vector2Int> pelletPositions = new List<Vector2Int>();
    private List<Vector2Int> powerPelletPositions = new List<Vector2Int>();

    // Ghost positions
    private List<Vector2Int> redGhostPositions = new List<Vector2Int>();
    private List<Vector2Int> cyanGhostPositions = new List<Vector2Int>();
    private List<Vector2Int> pinkGhostPositions = new List<Vector2Int>();
    private List<Vector2Int> orangeGhostPositions = new List<Vector2Int>();
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

        // Reset Positions
        pelletPositions = new List<Vector2Int>();
        powerPelletPositions = new List<Vector2Int>();
        ghostTraversablePositions = new List<Vector2Int>();
        redGhostPositions = new List<Vector2Int>();
        cyanGhostPositions = new List<Vector2Int>();
        pinkGhostPositions = new List<Vector2Int>();
        orangeGhostPositions = new List<Vector2Int>();

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

        Vector2Int pos = new Vector2Int(x, y);

        if (col.a == 0)
            return;
        if (col == wallColor)
            walls[x, y] = true;
        else if (col == pelletColor)
            pelletPositions.Add(pos);
        else if (col == powerPelletColor)
            powerPelletPositions.Add(pos);
        else if (col == ghostWallColor)
            ghostTraversablePositions.Add(pos);
        else if (col == redGhostColor)
            redGhostPositions.Add(pos);
        else if (col == cyanGhostColor)
            cyanGhostPositions.Add(pos);
        else if (col == pinkGhostColor)
            pinkGhostPositions.Add(pos);
        else if (col == orangeGhostColor)
            orangeGhostPositions.Add(pos);
        else if (col == playerColor)
            playerPosition = pos;
    }

    private void CreateMap()
    {
        GameObject map = new GameObject(mapTexture.name);
        map.AddComponent<GridbasedPathfinding>().CreateGrid(walls);
        CreateWalls(map.transform);
        CreateCollectables(map.transform);
        CreatePortals(map.transform);
        CreateMiniMap(map.transform);
        CreateGhosts(map.transform);
        CreateIntersections(map.transform);

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

        CreatePrefabWithHierarchy(collectables.transform, "Pellets", pellet, pelletPositions, "Pellet");
        CreatePrefabWithHierarchy(collectables.transform, "Power Pellets", powerPellet, powerPelletPositions, "Power Pellet");
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

    private void CreateGhosts(Transform parentTransform)
    {
        GameObject ghostHolder = new GameObject("Ghosts");
        ghostHolder.transform.parent = parentTransform;

        CreatePrefabWithHierarchy(ghostHolder.transform, "Red Ghosts (Blinky)", redGhost, redGhostPositions, "Blinky", false);
        CreatePrefabWithHierarchy(ghostHolder.transform, "Cyan Ghosts (Inky)", cyanGhost, cyanGhostPositions, "Inky", false);
        CreatePrefabWithHierarchy(ghostHolder.transform, "Pink Ghosts (Pinky)", pinkGhost, pinkGhostPositions, "Pinky", false);
        CreatePrefabWithHierarchy(ghostHolder.transform, "Orange Ghosts (Clyde)", orangeGhost, orangeGhostPositions, "Clyde", false);
    }

    private void CreatePrefabWithHierarchy(Transform parentTransform, string holderObjectName, GameObject prefab, List<Vector2Int> positions, string childObjectName, bool addOffset = true)
    {
        GameObject holder = new GameObject(holderObjectName);
        holder.transform.parent = parentTransform;
        CreatePrefabs(holder.transform, prefab, positions, childObjectName, addOffset);
    }

    private void CreatePrefabs(Transform prefabHolder, GameObject prefab, List<Vector2Int> pos, string name, bool addOffset = true)
    {
        for (int i = 0; i < pos.Count; i++)
        {
            CreatePrefab(prefabHolder, prefab, pos[i], $"{name} {i}", addOffset);
        }
    }

    private void CreatePrefab(Transform prefabHolder, GameObject prefab, Vector2Int pos, string name, bool addOffset = true)
    {
        GameObject pelletGO = Instantiate(prefab, IndexToPosition(pos) - (addOffset ? Vector3.up * tileSize / 4f : Vector3.zero), Quaternion.identity, prefabHolder);
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

    private void CreateIntersections(Transform parentTransform)
    {
        List<Vector2Int> intersectionPositions = new List<Vector2Int>();
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                if (IsIntersection(x, y))
                {
                    intersectionPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        CreatePrefabWithHierarchy(parentTransform, "Intersections", intersection, intersectionPositions, "Intersection", false);
    }

    private bool IsIntersection(int x, int y)
    {
        if (walls[x, y])
            return false;

        Vector2Int sum = Vector2Int.zero;
        int i = 0;

        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                // no need to check for exact match since the relative point is zero and therefore get canceled out
                if(nx == x || ny == y )
                {
                    if(!InMap(nx, ny) || InMap(nx, ny) && !walls[nx, ny])
                    {
                        sum += new Vector2Int(nx - x, ny - y);
                        i++;
                    }
                }
            }
        }

        // i == 5 because it takes mid into account
        return sum != Vector2Int.zero || i == 5;
    }

    private bool InMap(int x, int y) => x >= 0 && x < mapTexture.width && y >= 0 && y < mapTexture.height;

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

    public static List<Vector2Int> FindValidDirections(Vector2Int gridPosition)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int nx = gridPosition.x - 1; nx <= gridPosition.x + 1; nx++)
        {
            for (int ny = gridPosition.y - 1; ny <= gridPosition.y + 1; ny++)
            {
                if ((nx == gridPosition.x || ny == gridPosition.y) && current.InMap(nx, ny) && new Vector2Int(nx, ny) != gridPosition && !current.walls[nx, ny])
                {
                    result.Add(new Vector2Int(nx - gridPosition.x, ny - gridPosition.y));
                }
            }
        }

        return result;
    }

    public static Vector2Int RandomTile => new Vector2Int(Random.Range(0, current.mapTexture.width), Random.Range(0, current.mapTexture.height));
}
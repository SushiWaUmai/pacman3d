using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using NaughtyAttributes;
using UnityTimer;

public class GhostMovement : Movement
{
    public enum MovementMode
    {
        Chase,
        Scatter,
        Frightened, 
        Eaten
    }

    #region Variables

    [SerializeField] protected MovementMode current;
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;

    [SerializeField] private float minTileDistance = 0.1f;
    [ShowNonSerializedField] private bool isInIntersection;

    [SerializeField] private float modeChangeIntervall = 10; 
    [SerializeField] private float scatterConvergenceSpeed = 0.5f;
    [SerializeField] private float scatterProbability = 1;
    [SerializeField] protected Vector2 targetTile;
    [SerializeField] private float scatterTimeIntervall = 20;

    public bool IsEaten => current == MovementMode.Eaten;

    private GhostTargeting ghostTargeting;

    public event System.Action OnGhostEaten;
    public event System.Action OnGhostRecover;

    #endregion

    protected override void Init()
    {
        base.Init();
        this.AttachTimer(scatterTimeIntervall, SelectScatterTargetTile, null, true);
        this.AttachTimer(modeChangeIntervall, () => SwitchGhostMode(), null, true);
        OnPowerPelletCollect.AddListener(StartFrightenedMode);
        OnPowerPelletEnd.AddListener(EndFrightenedMode);
        ghostTargeting = GetComponent<GhostTargeting>();
    }

    protected override void Destruct()
    {
        base.Destruct();
        OnPowerPelletCollect.RemoveListener(StartFrightenedMode);
        OnPowerPelletEnd.RemoveListener(EndFrightenedMode);
    }

    private void OnTriggerStay(Collider other)
    {
        if (canInteract.Value)
        {
            if (other.gameObject.CompareTag("Intersection"))
            {
                if (!isInIntersection && (transform.position - other.gameObject.transform.position).sqrMagnitude < minTileDistance * minTileDistance)
                {
                    transform.position = other.transform.position;
                    InIntersection();
                    isInIntersection = true;
                }
            }
            else if(other.gameObject.CompareTag("GhostHouse"))
            {
                InGhostHouse();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Intersection"))
            isInIntersection = false;
    }

    private void InIntersection()
    {
        Vector2Int gridPosition = Map.PositionToIndex(transform.position);
        List<Vector2Int> validDirections = Map.FindValidDirections(gridPosition);
        // Remove the option to turn around by 180 degrees
        Vector2Int currentDir = Vector2Int.RoundToInt(new Vector2(dir.x, dir.z));
        if (validDirections.Contains(-currentDir) && validDirections.Count > 1)
            validDirections.Remove(-currentDir);


        switch (current)
        {
            case MovementMode.Chase:
                UpdateTargetTile(gridPosition);
                ShortestPathMovement(gridPosition, validDirections);
                break;
            case MovementMode.Scatter:
                ShortestPathMovement(gridPosition, validDirections);
                break;
            case MovementMode.Frightened:
                FrightenedMovement(validDirections);
                break;
            case MovementMode.Eaten:
                ShortestPathMovement(gridPosition, validDirections);
                break;

        }

        entityRotation = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

        MovementUpdate();
    }

    private void InGhostHouse()
    {
        if (current == MovementMode.Eaten)
        {
            SwitchGhostMode(true);
            OnGhostRecover.Invoke();
        }
    }

    #region Movement

    private void ShortestPathMovement(Vector2Int position, List<Vector2Int> validDirections)
    {
        Vector2Int bestDir = ShortestPath(validDirections, position, targetTile);
        ApplyDirection(bestDir);
    }

    private void FrightenedMovement(List<Vector2Int> validDirections)
    {
        Vector2Int selectedDir = validDirections[Random.Range(0, validDirections.Count)];
        ApplyDirection(selectedDir);
    }

    private void UpdateTargetTile(Vector2Int position)
    {
        Vector2? target = ghostTargeting.GetTargetTile(position);

        if (target != null)
            targetTile = (Vector2)target;
    }

    private Vector2Int ShortestPath(List<Vector2Int> directions, Vector2Int pos, Vector2 to)
    {
        float minSqrDistance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < directions.Count; i++)
        {
            Vector2Int current = directions[i] + pos;
            float sqrDist = (current - to).sqrMagnitude;
            if (minSqrDistance > sqrDist)
            {
                minSqrDistance = sqrDist;
                index = i;
            }
        }

        return directions[index];
    }

    private void ApplyDirection(Vector2Int direction) => dir = new Vector3(direction.x, 0, direction.y);

    private void InvertDirection()
    {
        if (!isInIntersection)
        {
            dir = -dir;
            entityRotation = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            MovementUpdate();
        }
    }

    private void SelectScatterTargetTile()
    {
        if(current == MovementMode.Scatter)
            targetTile = Map.RandomTile;
    }

    private void OnDrawGizmos()
    {
        if(ghostTargeting)
            Gizmos.DrawLine(transform.position, Map.IndexToPosition(Vector2Int.RoundToInt(targetTile)));
    }

    #endregion

    #region State Management

    private void StartFrightenedMode()
    {
        current = MovementMode.Frightened;
        InvertDirection();
    }

    private void EndFrightenedMode()
    {
        if(current != MovementMode.Eaten)
            SwitchGhostMode(true);
    }

    private void SwitchGhostMode(bool fromFrightenedOrEaten = false)
    {
        if(current == MovementMode.Chase || current == MovementMode.Scatter || fromFrightenedOrEaten)
        {
            MovementMode before = current;
            if(Random.value < scatterProbability)
            {
                current = MovementMode.Scatter;
                SelectScatterTargetTile();
                scatterProbability = Mathf.Lerp(scatterProbability, 0, scatterConvergenceSpeed);
            }
            else
            {
                current = MovementMode.Chase;
            }

            if(before != MovementMode.Eaten || before != MovementMode.Frightened)
            {
                if(before != current)
                {
                    InvertDirection();
                }
            }
        }
    }

    public void GetEaten()
    {
        current = MovementMode.Eaten;
        targetTile = Map.GetRandomGhostHousePosition();
        InvertDirection();

        OnGhostEaten.Invoke();
    }

    #endregion
}
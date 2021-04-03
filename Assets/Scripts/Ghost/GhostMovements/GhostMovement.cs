using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using NaughtyAttributes;
using UnityTimer;

public abstract class GhostMovement : Movement
{
    public enum MovementMode
    {
        Chase,
        Scatter,
        Frightened, 
        Eaten
    }

    [SerializeField] protected MovementMode current;
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;
    [SerializeField] private float minTileDistance = 0.1f;
    [ShowNonSerializedField] private bool isInIntersection;

    [SerializeField] private float modeChangeIntervall = 10; 
    [SerializeField] private float scatterConvergenceSpeed = 0.5f;
    [SerializeField] private float scatterProbability = 1;
    [SerializeField] private Vector2Int scatterTargetTile;
    [SerializeField] private float scatterTimeIntervall = 20;

    protected override void Init()
    {
        base.Init();
        this.AttachTimer(scatterTimeIntervall, () => scatterTargetTile = Map.RandomTile, null, true);
        this.AttachTimer(modeChangeIntervall, SwitchGhostMode, null, true);
        OnPowerPelletCollect.AddListener(StartFrightenedMode);
        OnPowerPelletEnd.AddListener(EndFrightenedMode);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Intersection"))
        {
            if (!isInIntersection && (transform.position - other.gameObject.transform.position).sqrMagnitude < minTileDistance * minTileDistance)
            {
                transform.position = other.transform.position;
                Vector2Int gridPosition = Map.PositionToIndex(transform.position);
                List<Vector2Int> validDirections = Map.FindValidDirections(gridPosition);
                // Remove the option to turn around by 180 degrees
                Vector2Int currentDir = Vector2Int.RoundToInt(new Vector2(dir.x, dir.z));
                if (validDirections.Contains(-currentDir) && validDirections.Count > 1)
                    validDirections.Remove(-currentDir);


                switch (current)
                {
                    case MovementMode.Chase:
                        CalculateChaseMovementDirection(gridPosition, validDirections);
                        break;
                    case MovementMode.Scatter:
                        ScatterMovement(gridPosition, validDirections);
                        break;
                    case MovementMode.Frightened:
                        FrightenedMovement(validDirections);
                        break;
                    case MovementMode.Eaten:
                        EatenMovement();
                        break;
                        
                }

                MovementUpdate();
                isInIntersection = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Intersection"))
            isInIntersection = false;
    }

    protected virtual void ScatterMovement(Vector2Int position, List<Vector2Int> validDirections)
    {
        Vector2Int bestDir = ShortestPath(validDirections, position, scatterTargetTile);
        ApplyDirection(bestDir);
    }

    protected virtual void FrightenedMovement(List<Vector2Int> validDirections)
    {
        Vector2Int selectedDir = validDirections[Random.Range(0, validDirections.Count)];
        ApplyDirection(selectedDir);
    }

    protected virtual void EatenMovement()
    {

    }

    // Calculates and applies the direction
    protected abstract void CalculateChaseMovementDirection(Vector2Int position, List<Vector2Int> validDirections);

    protected Vector2Int ShortestPath(List<Vector2Int> directions, Vector2Int pos, Vector2 to)
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

    protected void ApplyDirection(Vector2Int direction) => dir = new Vector3(direction.x, 0, direction.y);

    private void StartFrightenedMode()
    {
        current = MovementMode.Frightened;
        InvertDirection();
    }

    private void EndFrightenedMode()
    {
        SwitchGhostMode();
    }

    private void SwitchGhostMode()
    {
        MovementMode before = current;
        if(Random.value < scatterProbability)
        {
            current = MovementMode.Scatter;
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

    private void InvertDirection()
    {
        if (!isInIntersection)
        {
            dir = -dir;
            MovementUpdate();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;

public class PathFinder
{
    private TileType[,] grid;
    private readonly TileWeights tileWeights;
    private readonly int gridSize;
    private Dictionary<Vector2Int, Vector2Int> cameFrom;

    private static readonly Vector2Int[] DIRECTIONS = new[]
       {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    public PathFinder(TileType[,] grid, TileWeights tileWeights)
    {
        this.grid = grid;
        this.gridSize = grid.GetLength(0);
        this.tileWeights = tileWeights;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new();

        foreach (Vector2Int dir in DIRECTIONS)
        {
            Vector2Int newGridTile = current + dir;
            if (newGridTile.x < gridSize && newGridTile.y < gridSize && newGridTile.x >= 0 && newGridTile.y >= 0) neighbors.Add(newGridTile);
        }

        return neighbors;
    }

    private float GetCurrentCost(Vector2Int previous, Vector2Int next)
    {
        float weight = Mathf.Infinity;

        switch (grid[next.x, next.y])
        {
            case TileType.EMPTY:
                if (tileWeights.EMPTY == -1) weight = Mathf.Infinity;
                else weight = ((previous.x != next.x && previous.y != next.y) ? tileWeights.TURN_PENAILITY : 1) * tileWeights.EMPTY;
                break;
            case TileType.ROOM:
                if (tileWeights.ROOM == -1) weight = Mathf.Infinity;
                else weight = ((previous.x != next.x && previous.y != next.y) ? tileWeights.TURN_PENAILITY : 1) * tileWeights.ROOM;
                break;
            case TileType.HALLWAY:
                if (tileWeights.HALLWAY == -1) weight = Mathf.Infinity;
                else weight = tileWeights.HALLWAY;
                break;
            case TileType.HALLWAY_AND_ROOM:
                if (tileWeights.HALLWAY_AND_ROOM == -1) weight = Mathf.Infinity;
                else weight = tileWeights.HALLWAY_AND_ROOM;
                break;
            case TileType.ROOM_SPACING:
                if (tileWeights.TURN_PENAILITY == -1) weight = Mathf.Infinity;
                else weight = ((previous.x != next.x && previous.y != next.y) ? tileWeights.TURN_PENAILITY : 1) * tileWeights.ROOM_SPACING;
                break;
        }

        return weight;
    }

    private float GetManhattanDistance(Vector2Int goal, Vector2Int next)
    {
        return Mathf.Abs(goal.x - next.x) + Mathf.Abs(goal.y - next.y);
    }

    public Dictionary<Vector2Int, Vector2Int> FindOptimalPath(Vector2Int start, Vector2Int goal)
    {
        PriorityQueue<Vector2Int, float> fontier = new();
        Dictionary<Vector2Int, float> costSoFar = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();

        fontier.Enqueue(start, 0.0f);
        costSoFar[start] = 0.0f;

        while (fontier.Count != 0)
        {
            Vector2Int current = fontier.Dequeue();
            Vector2Int previous = (cameFrom.ContainsKey(current)) ? cameFrom[current] : current;

            if (current == goal) break;

            foreach (Vector2Int next in GetNeighbors(current))
            {
                float newCost = costSoFar[current] + GetCurrentCost(previous, next);
                if ((!costSoFar.ContainsKey(next) || newCost < costSoFar[next]))
                {
                    costSoFar[next] = newCost;
                    // Adds Heuristic Cost
                    float priority = newCost + GetManhattanDistance(goal, next);
                    fontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        this.cameFrom = cameFrom;

        return cameFrom;
    }

    public void DrawPath(Vector2Int start, float gridSize = 1, Color? color = null, float duration = Mathf.Infinity)
    {
        Vector2Int pt = start;
        while (true)
        {
            if (!cameFrom.ContainsKey(pt)) break;

            Vector3 worldPT = new Vector3(pt.x, 0, pt.y) * gridSize;
            Vector3 nextWorldPT = new Vector3(cameFrom[pt].x, 0, cameFrom[pt].y) * gridSize;

            Debug.DrawLine(worldPT, nextWorldPT, color ?? Color.magenta, duration);
            pt = cameFrom[pt];
        }
    }
}

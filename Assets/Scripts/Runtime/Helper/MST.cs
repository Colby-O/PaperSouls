using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST
{
    private readonly int numberOfVertices;
    private readonly float[,] adjacencyMatrix;
    private Dictionary<int, List<int>> parent;
    private readonly List<Vector2> vertexPositions;

    public MST(float[,] adjacencyMatrix, List<Vector2> vertexPositions)
    {
        this.numberOfVertices = adjacencyMatrix.GetLength(0);
        this.adjacencyMatrix = adjacencyMatrix;
        this.vertexPositions = vertexPositions;
    }

    private int GetMinimumKey(float[] key, bool[] inSet)
    {
        float min = Mathf.Infinity;
        int minIndex = -1;

        for (int v = 0; v < key.Length; v++)
        {
            if (!inSet[v] && key[v] < min)
            {
                min = key[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    public Dictionary<int, List<int>> GetMST()
    {
        parent = new Dictionary<int, List<int>>();

        float[] key = new float[numberOfVertices];
        bool[] inSet = new bool[numberOfVertices];

        for (int i = 0; i < numberOfVertices; i++)
        {
            parent.Add(i, new List<int>());
            key[i] = Mathf.Infinity;
            inSet[i] = false;
        }

        int firstIndex = Random.Range(0, numberOfVertices);
        key[firstIndex] = 0;

        for (int i = 0; i < numberOfVertices; i++)
        {
            int nextVertex = GetMinimumKey(key, inSet);

            if (nextVertex == -1) break;

            inSet[nextVertex] = true;

            for (int v = 0; v < numberOfVertices; v++)
            {
                if (adjacencyMatrix[nextVertex, v] != 0 && !inSet[v] && adjacencyMatrix[nextVertex, v] < key[v])
                {
                    if (parent[v].Count == 0) parent[v].Add(nextVertex);
                    else parent[v][0] = nextVertex;

                    key[v] = adjacencyMatrix[nextVertex, v];
                }
            }
        }

        return parent;
    }

    public void DrawMST(Color? color = null, float duration = Mathf.Infinity)
    {
        for (int i = 0; i < numberOfVertices; i++)
        {
            if (parent[i].Count == 0) continue;

            Vector3 ptParnet = new(vertexPositions[parent[i][0]].x, 0, vertexPositions[parent[i][0]].y);
            Vector3 pt = new(vertexPositions[i].x, 0, vertexPositions[i].y);

            Debug.DrawLine(ptParnet, pt, color ?? Color.red, duration);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge2D
{
    public Vector2 A, B;
    public int indexA, indexB;


    public Edge2D(Vector2 A, Vector2 B, int indexA, int indexB)
    {
        this.A = A;
        this.B = B;
        this.indexA = indexA;
        this.indexB = indexB;
    }

    public bool IsEqual(Edge2D other)
    {
        return (this.A == other.A && this.B == other.B) || (this.A == other.B && this.B == other.A);
    }

}

public struct Triangle2D
{
    public Vector2 A, B, C;
    public int indexA, indexB, indexC;

    public Triangle2D(Vector2 A, Vector2 B, Vector2 C, int indexA, int indexB, int indexC)
    {
        this.A = A;
        this.B = B;
        this.C = C;
        this.indexA = indexA;
        this.indexB = indexB;
        this.indexC = indexC;
    }

    public void DrawTriangle(Color? color = null, float duration = Mathf.Infinity)
    {
        Debug.DrawLine(new Vector3(this.A.x, 0.0f, this.A.y), new Vector3(this.B.x, 0.0f, this.B.y), color ?? Color.green, duration);
        Debug.DrawLine(new Vector3(this.A.x, 0.0f, this.A.y), new Vector3(this.C.x, 0.0f, this.C.y), color ?? Color.green, duration);
        Debug.DrawLine(new Vector3(this.B.x, 0.0f, this.B.y), new Vector3(this.C.x, 0.0f, this.C.y), color ?? Color.green, duration);
    }
}

public class DelaunayTriangulation 
{

    private readonly List<Vector2> vertices;
    private readonly List<Triangle2D> triangulation;

    public DelaunayTriangulation(List<Vector2> vertices)
    {
        this.vertices = vertices;
        triangulation = new();
    }

    private Triangle2D CalaulateSuperTriangle()
    {
        Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity);
        Vector2 max = new Vector2(-Mathf.Infinity, -Mathf.Infinity);

        foreach (Vector2 vertex in vertices)
        {
            min.x = Mathf.Min(min.x, vertex.x);
            min.y = Mathf.Min(min.y, vertex.y);
            max.x = Mathf.Max(max.x, vertex.x);
            max.y = Mathf.Max(max.y, vertex.y);
        }

        float dx = (max.x - min.x) * 10.0f;
        float dy = (max.y - min.y) * 10.0f;

        return new Triangle2D(new Vector2(min.x - dx, min.y - dy * 3), new Vector2(min.x - dx, max.y + dy), new Vector2(max.x + dx * 3, max.y + dy), -1, -1, -1);
    }

    private bool IsPointInCircumcircle(Triangle2D triangle, Vector2 pt)
    {
        float dx = triangle.A.x - pt.x;
        float dy = triangle.A.y - pt.y;
        float ex = triangle.B.x - pt.x;
        float ey = triangle.B.y - pt.y;
        float fx = triangle.C.x - pt.x;
        float fy = triangle.C.y - pt.y;

        float ap = dx * dx + dy * dy;
        float bp = ex * ex + ey * ey;
        float cp = fx * fx + fy * fy;

        return dx * (ey * cp - bp * fy) - dy * (ex * cp - bp * fx) + ap * (ex * fy - ey * fx) < 0;
    }

    private List<Edge2D> FindUniqueEdges(List<Edge2D> edges)
    {
        List<Edge2D> uniqueEdges = new List<Edge2D>();

        for (int i = 0; i < edges.Count; i++)
        {
            bool isUnique = true;

            for (int j = 0; j < edges.Count; j++)
            {
                if (i != j && edges[i].IsEqual(edges[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique) uniqueEdges.Add(edges[i]);
        }

        return uniqueEdges;
    }

    private void AddVertex(Vector2 vertex, int roomID)
    {
        List<Edge2D> edges = new();

        for (int i = triangulation.Count - 1; i > -1; i--)
        {
            if (IsPointInCircumcircle(triangulation[i], vertex))
            {
                edges.Add(new Edge2D(triangulation[i].A, triangulation[i].B, triangulation[i].indexA, triangulation[i].indexB));
                edges.Add(new Edge2D(triangulation[i].B, triangulation[i].C, triangulation[i].indexB, triangulation[i].indexC));
                edges.Add(new Edge2D(triangulation[i].C, triangulation[i].A, triangulation[i].indexC, triangulation[i].indexA));
                triangulation.RemoveAt(i);
            }
        }

        List<Edge2D> uniqueEdges = FindUniqueEdges(edges);

        foreach (Edge2D edge in uniqueEdges)
        {
            triangulation.Add(new Triangle2D(edge.A, edge.B, vertex, edge.indexA, edge.indexB, roomID));
        }
    }

    private bool HasSharedEdagesWithSuper(Triangle2D triangle, Triangle2D superTriangle)
    {
        return (triangle.A == superTriangle.A || triangle.A == superTriangle.B || triangle.A == superTriangle.C || triangle.B == superTriangle.A || triangle.B == superTriangle.B || triangle.B == superTriangle.C || triangle.C == superTriangle.A || triangle.C == superTriangle.B || triangle.C == superTriangle.C);
    }

    private float[,] GenerateAdjacencyMatrix()
    {
        float[,] adjacencyMatrix = new float[vertices.Count, vertices.Count];

        foreach (Triangle2D triangle in triangulation)
        {
            adjacencyMatrix[triangle.indexA, triangle.indexB] = Vector2.Distance(triangle.A, triangle.B);
            adjacencyMatrix[triangle.indexB, triangle.indexA] = Vector2.Distance(triangle.B, triangle.A);
            adjacencyMatrix[triangle.indexB, triangle.indexC] = Vector2.Distance(triangle.B, triangle.C);
            adjacencyMatrix[triangle.indexC, triangle.indexB] = Vector2.Distance(triangle.C, triangle.B);
            adjacencyMatrix[triangle.indexC, triangle.indexA] = Vector2.Distance(triangle.C, triangle.A);
            adjacencyMatrix[triangle.indexA, triangle.indexC] = Vector2.Distance(triangle.A, triangle.C);
        }

        return adjacencyMatrix;
    }

    public float[,] CalculateDelaunayTriangulation()
    {
        Triangle2D superTriangle = CalaulateSuperTriangle();
        triangulation.Add(superTriangle);

        for (int i = 0; i < vertices.Count; i++)
        {
            AddVertex(vertices[i], i);
        }

        triangulation.RemoveAll(triangle => HasSharedEdagesWithSuper(triangle, superTriangle));

        return GenerateAdjacencyMatrix();
    }

    public void DrawDelaunayTriangulation(Color? color = null, float duration = Mathf.Infinity)
    {
        foreach (Triangle2D triangle in triangulation)
        {
            triangle.DrawTriangle(color, duration);
        }
    }
}

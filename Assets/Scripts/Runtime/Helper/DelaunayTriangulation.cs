using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Helpers
{
    /// <summary>
    /// Define a 2D edge
    /// </summary>
    internal struct Edge2D
    {
        public Vector2 A, B;
        public int IndexA, IndexB;

        /// <summary>
        /// Constructor take two ponts in space and there indeices in the overall vertex list
        /// </summary>
        public Edge2D(Vector2 A, Vector2 B, int indexA, int indexB)
        {
            this.A = A;
            this.B = B;
            this.IndexA = indexA;
            this.IndexB = indexB;
        }

        /// <summary>
        /// Check if two edges are equal
        /// </summary>
        public bool IsEqual(Edge2D other)
        {
            return (this.A == other.A && this.B == other.B) || (this.A == other.B && this.B == other.A);
        }

    }

    /// <summary>
    /// Define a 2D triangle
    /// </summary>
    internal struct Triangle2D
    {
        public Vector2 A, B, C;
        public int IndexA, IndexB, IndexC;

        /// <summary>
        /// Constructor take three ponts in space and there indeices in the overall vertex list
        /// </summary>
        public Triangle2D(Vector2 A, Vector2 B, Vector2 C, int indexA, int indexB, int indexC)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.IndexA = indexA;
            this.IndexB = indexB;
            this.IndexC = indexC;
        }

        /// <summary>
        /// Draws a trangle for debugging
        /// </summary>
        public void DrawTriangle(Color? color = null, float duration = Mathf.Infinity)
        {
            Debug.DrawLine(new Vector3(this.A.x, 0.0f, this.A.y), new Vector3(this.B.x, 0.0f, this.B.y), color ?? Color.green, duration);
            Debug.DrawLine(new Vector3(this.A.x, 0.0f, this.A.y), new Vector3(this.C.x, 0.0f, this.C.y), color ?? Color.green, duration);
            Debug.DrawLine(new Vector3(this.B.x, 0.0f, this.B.y), new Vector3(this.C.x, 0.0f, this.C.y), color ?? Color.green, duration);
        }
    }

    internal sealed class DelaunayTriangulation
    {

        private readonly List<Vector2> _vertices;
        private readonly List<Triangle2D> _triangulation;

        /// <summary>
        /// Consturction take an a list of vertex positions
        /// </summary>
        public DelaunayTriangulation(List<Vector2> vertices)
        {
            this._vertices = vertices;
            _triangulation = new();
        }

        /// <summary>
        /// Calculates a "super" triangle enclosing all points in the vertex list
        /// </summary>
        private Triangle2D CalaulateSuperTriangle()
        {
            Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 max = new Vector2(-Mathf.Infinity, -Mathf.Infinity);

            foreach (Vector2 vertex in _vertices)
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

        /// <summary>
        /// Check if a point lies within in a circumcircle
        /// </summary>
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

        /// <summary>
        /// Find all unique edges in an edge list
        /// </summary>
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

        /// <summary>
        /// Adds a vertex to the triangulation
        /// </summary>
        private void AddVertex(Vector2 vertex, int roomID)
        {
            List<Edge2D> edges = new();

            for (int i = _triangulation.Count - 1; i > -1; i--)
            {
                if (IsPointInCircumcircle(_triangulation[i], vertex))
                {
                    edges.Add(new Edge2D(_triangulation[i].A, _triangulation[i].B, _triangulation[i].IndexA, _triangulation[i].IndexB));
                    edges.Add(new Edge2D(_triangulation[i].B, _triangulation[i].C, _triangulation[i].IndexB, _triangulation[i].IndexC));
                    edges.Add(new Edge2D(_triangulation[i].C, _triangulation[i].A, _triangulation[i].IndexC, _triangulation[i].IndexA));
                    _triangulation.RemoveAt(i);
                }
            }

            List<Edge2D> uniqueEdges = FindUniqueEdges(edges);

            foreach (Edge2D edge in uniqueEdges)
            {
                _triangulation.Add(new Triangle2D(edge.A, edge.B, vertex, edge.IndexA, edge.IndexB, roomID));
            }
        }

        /// <summary>
        /// Checks if a two triangles have a shared edge
        /// </summary>
        private bool HasSharedEdages(Triangle2D triangle, Triangle2D otherTriangle)
        {
            return (triangle.A == otherTriangle.A || triangle.A == otherTriangle.B || triangle.A == otherTriangle.C || triangle.B == otherTriangle.A || triangle.B == otherTriangle.B || triangle.B == otherTriangle.C || triangle.C == otherTriangle.A || triangle.C == otherTriangle.B || triangle.C == otherTriangle.C);
        }

        /// <summary>
        /// Generate an Adjacency Matrix from a triangulation
        /// </summary>
        private float[,] GenerateAdjacencyMatrix()
        {
            float[,] adjacencyMatrix = new float[_vertices.Count, _vertices.Count];

            foreach (Triangle2D triangle in _triangulation)
            {
                adjacencyMatrix[triangle.IndexA, triangle.IndexB] = Vector2.Distance(triangle.A, triangle.B);
                adjacencyMatrix[triangle.IndexB, triangle.IndexA] = Vector2.Distance(triangle.B, triangle.A);
                adjacencyMatrix[triangle.IndexB, triangle.IndexC] = Vector2.Distance(triangle.B, triangle.C);
                adjacencyMatrix[triangle.IndexC, triangle.IndexB] = Vector2.Distance(triangle.C, triangle.B);
                adjacencyMatrix[triangle.IndexC, triangle.IndexA] = Vector2.Distance(triangle.C, triangle.A);
                adjacencyMatrix[triangle.IndexA, triangle.IndexC] = Vector2.Distance(triangle.A, triangle.C);
            }

            return adjacencyMatrix;
        }

        /// <summary>
        /// Computes the Delaunay Triangulation of a set of vertices and return the Adjacency Matrix 
        /// of the underlying graph.
        /// </summary>
        public float[,] CalculateDelaunayTriangulation()
        {
            Triangle2D superTriangle = CalaulateSuperTriangle();
            _triangulation.Add(superTriangle);

            for (int i = 0; i < _vertices.Count; i++)
            {
                AddVertex(_vertices[i], i);
            }

            _triangulation.RemoveAll(triangle => HasSharedEdages(triangle, superTriangle));

            return GenerateAdjacencyMatrix();
        }

        /// <summary>
        /// Draws the Delaunay Triangulation for debugging
        /// </summary>
        public void DrawDelaunayTriangulation(Color? color = null, float duration = Mathf.Infinity)
        {
            foreach (Triangle2D triangle in _triangulation)
            {
                triangle.DrawTriangle(color, duration);
            }
        }
    }
}

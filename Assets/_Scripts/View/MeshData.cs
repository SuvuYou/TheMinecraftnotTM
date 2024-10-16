using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public InternalMeshData MainMesh = new();
    public InternalMeshData WaterMesh = new();
}

public class InternalMeshData
{
    public List<Vector3> Vertices { get; private set; } = new ();
    public List<int> Triangles { get; private set; } = new ();
    public List<Vector2> UV { get; private set; } = new ();

    public List<Vector3> ColliderVertices { get; private set; } = new ();
    public List<int> ColliderTriangles { get; private set; } = new ();

    public void AddVertex(Vector3 position, bool isGeneratingCollider)
    {
        Vertices.Add(position);

        if (isGeneratingCollider)
        {
            ColliderVertices.Add(position);
        }
    }

    public void AddQuadTriangles(bool isGeneratingCollider)
    {
        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 3);
        Triangles.Add(Vertices.Count - 2);
        
        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 2);
        Triangles.Add(Vertices.Count - 1);

        if (isGeneratingCollider)
        {
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 3);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 1);
        }
    }
}

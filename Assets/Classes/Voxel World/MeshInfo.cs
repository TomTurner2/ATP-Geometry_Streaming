using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfo
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> indices = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    public List<Vector3> collider_vertices = new List<Vector3>();
    public List<int> collider_indices = new List<int>();
    public bool collider_from_mesh = true;
    public bool collider_only = false;


    public void AddVertex(Vector3 _vertex)
    {
        if (!collider_only)
            vertices.Add(_vertex);

        if (collider_from_mesh)
            collider_vertices.Add(_vertex);//if using same mesh for collider
    }


    public void AddFaceIndices()
    {
        if (!collider_only)
        {
            indices.Add(vertices.Count - 4);
            indices.Add(vertices.Count - 3);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 4);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 1);
        }

        if (!collider_from_mesh)
            return;

        collider_indices.Add(collider_vertices.Count - 4);
        collider_indices.Add(collider_vertices.Count - 3);
        collider_indices.Add(collider_vertices.Count - 2);
        collider_indices.Add(collider_vertices.Count - 4);
        collider_indices.Add(collider_vertices.Count - 2);
        collider_indices.Add(collider_vertices.Count - 1);
    }
}

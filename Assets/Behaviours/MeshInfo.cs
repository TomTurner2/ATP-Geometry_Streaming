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
    public bool collider_from_mesh;


    public void AddVertex(Vector3 _vertex)
    {
        vertices.Add(_vertex);
        if (collider_from_mesh)
            collider_vertices.Add(_vertex);//if using same mesh for collider
    }


    public void AddIndice(int _indice)
    {
        indices.Add(_indice);
        if (collider_from_mesh)
            collider_indices.Add(_indice - (vertices.Count - collider_vertices.Count));//adjust to match vert indexs
    }


    public void AddFaceIndices()
    {
        indices.Add(vertices.Count - 4);
        indices.Add(vertices.Count - 3);
        indices.Add(vertices.Count - 2);
        indices.Add(vertices.Count - 4);
        indices.Add(vertices.Count - 2);
        indices.Add(vertices.Count - 1);

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

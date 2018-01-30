using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PolygonGenerator : MonoBehaviour
{
    [Tooltip("How many rows and columns for the whole texture, " +
             "including unfilled slots")]
    [SerializeField] Vector2 tile_map_grid_size = new Vector2(8, 8);

    [Space]
    [Header("Block Grid Coords")]
    [SerializeField] Vector2 stone_coords = new Vector2(1, 1);
    [SerializeField] Vector2 grass_coords = new Vector2(1, 3);

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> indices = new List<int>();
    private Mesh mesh;


	private void Start ()
	{
	    mesh = GetComponent<MeshFilter>().mesh;
        GeneratePolys();
	}


    private void GeneratePolys()
    {
        CreateVertices();
        CreateIndicies();
        CreateUVs();
        CreateMesh();
    }


    private void CreateVertices()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        //create square corner points
        verticies.Add(new Vector3(x, y, z));
        verticies.Add(new Vector3(x + 1, y, z));
        verticies.Add(new Vector3(x + 1, y - 1, z));
        verticies.Add(new Vector3(x, y - 1, z));
    }


    private void CreateIndicies()
    {
        indices.Add(0);
        indices.Add(1);
        indices.Add(3);
        indices.Add(1);
        indices.Add(2);
        indices.Add(3);
    }


    private void CreateUVs()
    {
        //tile size as a percentage of the tilemap dimensions
        float tex_x = 0;
        float tex_y = 0;
        GetInverseSize(out tex_x, out tex_y);
 
        //create UV coords
        uvs.Add(new Vector2(tex_x * stone_coords.x,
            tex_y * (stone_coords.y - 1) + tex_y));
        uvs.Add(new Vector2(tex_x * stone_coords.x + tex_x,
            tex_y * (stone_coords.y - 1) + tex_y));
        uvs.Add(new Vector2(tex_x * stone_coords.x + tex_x,
            tex_y * (stone_coords.y - 1)));
        uvs.Add(new Vector2(tex_x * stone_coords.x,
            tex_y * (stone_coords.y - 1)));
    }


    private void GetInverseSize(out float _inverse_x, out float _inverse_y)
    {
        _inverse_x = 1.0f / tile_map_grid_size.x;
        _inverse_y = 1.0f / tile_map_grid_size.y;
    }


    private void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        MeshUtility.Optimize(mesh);
        mesh.RecalculateNormals();
    }


}

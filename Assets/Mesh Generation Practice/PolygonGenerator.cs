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

    [Space]
    [SerializeField] byte[,] voxels;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> indices = new List<int>();
    private int square_count = 0;
    private Mesh mesh;


	private void Start ()
	{
	    mesh = GetComponent<MeshFilter>().mesh;
        CreateTerrain();
	}


    //Creates a single polygon
    private void CreatePoly(int _x, int _y, Vector2 _tex_coord)
    {
        CreateSquare(_x, _y, _tex_coord);
        UpdateMesh();
        CleanUp();
    }


    //Creates square data
    private void CreateSquare(int _x, int _y, Vector2 _texture)
    {
        CreateVertices(_x, _y);
        CreateIndicies();
        CreateUVs(_texture);
        ++square_count;
    }


    //Creates vertex positions of a square
    private void CreateVertices(int _x, int _y)
    {
        //create square corner points
        verticies.Add(new Vector3(_x, _y, 0));
        verticies.Add(new Vector3(_x + 1, _y, 0));
        verticies.Add(new Vector3(_x + 1, _y - 1, 0));
        verticies.Add(new Vector3(_x, _y - 1, 0));
    }


    //Creates indices to describe triangles
    private void CreateIndicies()
    {
        indices.Add(square_count * 4);
        indices.Add((square_count * 4) + 1);
        indices.Add((square_count * 4) + 3);
        indices.Add((square_count * 4) + 1);
        indices.Add((square_count * 4) + 2);
        indices.Add((square_count * 4) + 3);
    }


    //Creates UV mapping
    private void CreateUVs(Vector2 _coords)
    {
        float tex_x = 0;
        float tex_y = 0;
        GetInverseSize(out tex_x, out tex_y);
 
        //create UV coords
        uvs.Add(new Vector2(tex_x * _coords.x,
            tex_y * (_coords.y - 1) + tex_y));
        uvs.Add(new Vector2(tex_x * _coords.x + tex_x,
            tex_y * (_coords.y - 1) + tex_y));
        uvs.Add(new Vector2(tex_x * _coords.x + tex_x,
            tex_y * (_coords.y - 1)));
        uvs.Add(new Vector2(tex_x * _coords.x,
            tex_y * (_coords.y - 1)));
    }


    //Calculates tile size as percentage
    private void GetInverseSize(out float _inverse_x, out float _inverse_y)
    {
        _inverse_x = 1.0f / tile_map_grid_size.x;
        _inverse_y = 1.0f / tile_map_grid_size.y;
    }


    //Updates the mesh using lists
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        MeshUtility.Optimize(mesh);
        mesh.RecalculateNormals();
    }


    //Clears lists and resets square count
    private void CleanUp()
    {
        square_count = 0;
        verticies.Clear();
        indices.Clear();
        uvs.Clear();
    }


    //Creates terrain and updates mesh
    private void CreateTerrain()
    {
        CreateTerrainData();
        CreateTerrainMesh();
        UpdateMesh();
        CleanUp();
    }


    //Creates the terrain data
    private void CreateTerrainData()
    {
        voxels = new byte[10, 10];

        for (int x = 0; x < voxels.GetLength(0); ++x)
        {
            for (int y = 0; y < voxels.GetLength(1); ++y)
            {
                if (y == 5)
                {
                    voxels[x, y] = 2;
                }
                else if (y < 5)
                {
                    voxels[x, y] = 1;
                }
            }
        }
    }


    //Creates terrain mesh using data
    private void CreateTerrainMesh()
    {
        for (int x = 0; x < voxels.GetLength(0); ++x)
        {
            for (int y = 0; y < voxels.GetLength(1); ++y)
            {
                switch (voxels[x, y])
                {
                    case 1: CreateSquare(x, y, stone_coords);
                        break;
                    case 2: CreateSquare(x, y, grass_coords);
                        break;
                }
            }
        }
    }
}

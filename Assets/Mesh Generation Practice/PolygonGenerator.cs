using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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


    private byte[,] voxels;
    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> indices = new List<int>();
    private int square_count = 0;
    private Mesh mesh;


    private List<Vector3> collider_vertices = new List<Vector3>();
    private List<int> collider_indices = new List<int>();
    private int collider_count = 0;
    private MeshCollider mesh_collider;


	private void Start ()
	{
	    mesh = GetComponent<MeshFilter>().mesh;
	    mesh_collider = GetComponent<MeshCollider>();
        CreateTerrain();
	}


    //Creates a single polygon
    private void CreatePoly(int _x, int _y, Vector2 _tex_coord)
    {
        CreateSquare(_x, _y, _tex_coord);
        UpdateMesh();
        MeshCleanUp();
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
    private void MeshCleanUp()
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
        MeshCleanUp();

        CreateCollider();
        ColliderCleanUp();
    }


    //Creates the terrain data
    private void CreateTerrainData()
    {
        voxels = new byte[96, 128];

        for (int x = 0; x < voxels.GetLength(0); ++x)
        {
            int stone = Noise(x, 0, 80, 15, 1);
            stone += Noise(x, 0, 50, 30, 1);
            stone += Noise(x, 0, 10, 10, 1);
            stone += 75;

            int dirt = Noise(x, 0, 100, 35, 1);
            dirt += Noise(x, 0, 50, 30, 1);
            dirt += 75;

            for (int y = 0; y < voxels.GetLength(1); ++y)
            {
                if (y < stone)
                {
                    voxels[x, y] = 1;

                    if (Noise(x, y, 12, 16, 1) > 10)
                    {
                        voxels[x, y] = 2;
                    }

                    if (Noise(x, y * 2, 16, 14, 1) > 10)
                    {
                        voxels[x, y] = 0;
                    }
                }
                else if (y < dirt)
                {
                    voxels[x, y] = 2;
                }
            }
        }
    }


    private int Noise(int _x, int _y, float _scale, float _magnitude, float _exp)
    {
        return (int) (Mathf.Pow((Mathf.PerlinNoise(_x / _scale, _y / _scale) * _magnitude), (_exp)));
    }


    private void SaveVoxelData()
    {
        string file_name = "New World.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(file_name, FileMode.Create, FileAccess.Write);
        bf.Serialize(fs, voxels);
    }


    //Creates terrain mesh using data
    private void CreateTerrainMesh()
    {
        for (int x = 0; x < voxels.GetLength(0); ++x)
        {
            for (int y = 0; y < voxels.GetLength(1); ++y)
            {
                if (voxels[x, y] == 0)
                    continue;

                CreateSquareCollider(x, y);

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


    private void CreateSquareCollider(int _x, int _y)
    {
        //top collider
        if (CheckVoxel(_x, _y + 1) == 0)
        {
            collider_vertices.Add(new Vector3(_x, _y, 1));
            collider_vertices.Add(new Vector3(_x + 1, _y, 1));
            collider_vertices.Add(new Vector3(_x + 1, _y, 0));
            collider_vertices.Add(new Vector3(_x, _y, 0));
            ColliderIndices();
            ++collider_count;
        }

        //bottom
        if (CheckVoxel(_x, _y - 1) == 0)
        {
            collider_vertices.Add(new Vector3(_x, _y - 1, 0));
            collider_vertices.Add(new Vector3(_x + 1, _y - 1, 0));
            collider_vertices.Add(new Vector3(_x + 1, _y - 1, 1));
            collider_vertices.Add(new Vector3(_x, _y - 1, 1));
            ColliderIndices();
            ++collider_count;
        }

        //left
        if (CheckVoxel(_x - 1, _y) == 0)
        {
            collider_vertices.Add(new Vector3(_x, _y - 1, 1));
            collider_vertices.Add(new Vector3(_x, _y, 1));
            collider_vertices.Add(new Vector3(_x, _y, 0));
            collider_vertices.Add(new Vector3(_x, _y - 1, 0));
            ColliderIndices();
            ++collider_count;
        }

        //right
        if (CheckVoxel(_x + 1, _y) == 0)
        {
            collider_vertices.Add(new Vector3(_x + 1, _y, 1));
            collider_vertices.Add(new Vector3(_x + 1, _y - 1, 1));
            collider_vertices.Add(new Vector3(_x + 1, _y - 1, 0));
            collider_vertices.Add(new Vector3(_x + 1, _y, 0));
            ColliderIndices();
            ++collider_count;
        }
    }

    byte CheckVoxel(int _x, int _y)
    {
        if (_x == -1 || _x == voxels.GetLength(0) || _y == -1 || _y == voxels.GetLength(1))
        {
            return (byte)1;
        }

        return voxels[_x, _y];
    }


    private void ColliderIndices()
    {
        collider_indices.Add(collider_count * 4);
        collider_indices.Add((collider_count * 4) + 1);
        collider_indices.Add((collider_count * 4) + 3);
        collider_indices.Add((collider_count * 4) + 1);
        collider_indices.Add((collider_count * 4) + 2);
        collider_indices.Add((collider_count * 4) + 3);
    }


    private void CreateCollider()
    {
        Mesh collider_mesh = new Mesh();
        collider_mesh.vertices = collider_vertices.ToArray();
        collider_mesh.triangles = collider_indices.ToArray();
        mesh_collider.sharedMesh = collider_mesh;
    }


    private void ColliderCleanUp()
    {
        collider_vertices.Clear();
        collider_indices.Clear();
        collider_count = 0;
    }
}

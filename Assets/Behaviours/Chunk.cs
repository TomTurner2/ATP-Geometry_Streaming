using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static int chunk_size = 16;
    public Voxel[,,] voxels = new Voxel[chunk_size, chunk_size, chunk_size];
    public VoxelWorld voxel_world;
    public intVector3 voxel_world_position;
    public bool edited = false;//also used to determine if chunk needs updating
    public bool rendered = false;

    private MeshFilter mesh_filter;
    private MeshCollider mesh_collider;


    void Start()
    {
        mesh_filter = gameObject.GetComponent<MeshFilter>();
        mesh_collider = gameObject.GetComponent<MeshCollider>();

    }


    void Update()
    {
        if (!edited)
            return;

        edited = false;
        UpdateChunk();
    }


    public Voxel GetVoxel(int _x, int _y, int _z)
    {
        if (VoxelInChunk(_x, _y, _z))//if voxel is in this chunk return it
            return voxels[_x, _y, _z];

        return voxel_world.GetBlock(voxel_world_position.x + _x, voxel_world_position.y +
            _y, voxel_world_position.z + _z);//if voxel not in chunk find it through world
    }


    public static bool VoxelInChunk(int _x, int _y, int _z)
    {
        return ValidateCoordinate(_x) && ValidateCoordinate(_y) && ValidateCoordinate(_z);
    }


    public static bool ValidateCoordinate(int _index)
    {
        return _index >= 0 && _index < chunk_size;
    }


    public void SetVoxel(int _x, int _y, int _z, Voxel _voxel)
    {
        if (VoxelInChunk(_x, _y, _z))//if in chunk
        {
            voxels[_x, _y, _z] = _voxel;//set voxel
        }
        else
        {
            voxel_world.SetBlock(voxel_world_position.x + _x, voxel_world_position.y +
                _y, voxel_world_position.z + _z, _voxel);//if not in chunk set voxel through world
        }
    }


    void UpdateChunk()
    {
        rendered = true;

        MeshInfo mesh_info = new MeshInfo();
        for (int x = 0; x < chunk_size; x++)
        {
            for (int y = 0; y < chunk_size; y++)
            {
                for (int z = 0; z < chunk_size; z++)
                {
                    mesh_info = voxels[x, y, z].GetVoxelMeshInfo(this, x, y, z, mesh_info);//Get voxels mesh info
                }
            }
        }
        RenderMesh(mesh_info);
    }


    void RenderMesh(MeshInfo _mesh_info)
    {
        UpdateMesh(_mesh_info);
        UpdateCollider(_mesh_info);   
    }


    private void UpdateMesh(MeshInfo _mesh_info)
    {
        mesh_filter.mesh.Clear();
        mesh_filter.mesh.vertices = _mesh_info.vertices.ToArray();
        mesh_filter.mesh.triangles = _mesh_info.indices.ToArray();
        mesh_filter.mesh.uv = _mesh_info.uvs.ToArray();
        mesh_filter.mesh.RecalculateNormals();
    }


    private void UpdateCollider(MeshInfo _mesh_info)
    {
        mesh_collider.sharedMesh = null;
        Mesh mesh = new Mesh
        {
            vertices = _mesh_info.collider_vertices.ToArray(),
            triangles = _mesh_info.collider_indices.ToArray()
        };
        mesh.RecalculateNormals();
        mesh_collider.sharedMesh = mesh;
    }


    public void SetVoxelsUnEdited()
    {
        foreach (Voxel voxel in voxels)
        {
            voxel.edited = false;
        }
    }


    private void OnDrawGizmos()
    {
        float offset = chunk_size * 0.5f - 0.5f;
        Gizmos.DrawWireCube(voxel_world_position.ToVector3() + Vector3.one * offset,
            new Vector3(chunk_size, chunk_size, chunk_size));
    }
}

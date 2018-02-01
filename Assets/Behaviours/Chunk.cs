using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private byte[,,] data;
    private Mesh mesh;
    private VoxelMeshGenerator voxel_mesh_generator;


    public void LoadChunk(byte[,,] _data, VoxelMeshGenerator _voxel_mesh_generator)
    {
        data = _data;
        voxel_mesh_generator = _voxel_mesh_generator;
        CreateMesh();
        UpdateMesh();
    }


    private void CreateMesh()
    {
        mesh = new Mesh();
    }


    public void UpdateMesh()
    {
        if (voxel_mesh_generator == null)
            return;

        voxel_mesh_generator.GenerateMesh(data, ref mesh);
    }


    byte[,,] GetData()
    {
        return data;
    }

}

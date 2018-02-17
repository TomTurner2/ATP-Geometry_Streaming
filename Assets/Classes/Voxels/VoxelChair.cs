using System;
using UnityEngine;


[Serializable]
public class VoxelChair : Voxel
{
    [NonSerialized] private static GameObject chair_mesh_prefab = null;
    [NonSerialized] private GameObject mesh_instance = null;


    public override MeshInfo GetVoxelMeshInfo(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.collider_only = true;
        base.GetVoxelMeshInfo(_chunk, _x, _y, _z, _mesh_info);
        _mesh_info.collider_only = false;

        if (chair_mesh_prefab == null)
            chair_mesh_prefab = MeshManager.instance.GetMeshPrefabByName("Chair");

        mesh_instance = GameObject.Instantiate(chair_mesh_prefab, _chunk.transform);
        mesh_instance.transform.position = _chunk.voxel_world_position + new intVector3(_x, _y, _z);

        return _mesh_info;
    }


    public override bool IsSolid()
    {
        return true;
    }


    public override GameObject OnDestroy()
    {
        return mesh_instance;
    }


    public override bool HasCustomMesh()
    {
        return true;
    }

}

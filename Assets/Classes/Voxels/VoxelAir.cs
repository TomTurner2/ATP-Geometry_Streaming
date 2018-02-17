using System;


[Serializable]
public class VoxelAir : Voxel
{
    public override MeshInfo GetVoxelMeshInfo(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        return _mesh_info;
    }


    public override bool IsSolid()
    {
        return false;
    }
}

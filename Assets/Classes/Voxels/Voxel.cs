using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class Voxel
{
    public bool edited = true;


    public virtual MeshCreator.TileTexture GetTextureCoordsByDirection(MeshCreator.Direction _direction)//base defaults to first tile
    {
        MeshCreator.TileTexture tile_texture = new MeshCreator.TileTexture
        {
            x = 0,
            y = 0
        };
        return tile_texture;
    }


    public virtual MeshInfo GetVoxelMeshInfo (Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        return MeshCreator.GenerateMesh(_chunk, _x, _y, _z, _mesh_info, this);
    }


    public virtual bool IsSolid()
    {
        return true;//solid by default
    }


    public virtual bool HasCustomMesh()
    {
        return false;
    }


    public virtual GameObject OnDestroy()
    {
        return null;
    }


    public virtual Voxel Clone()
    {
        return this;
    }

}

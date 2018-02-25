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


    public virtual Vector2[] GetFaceUVs(Direction _direction)
    {
        Vector2[] uvs = new Vector2[4];
        TileTexture tile_texture_pos = GetTextureCoordsByDirection(_direction);

        //Calculate uvs based on tile coordinates
        uvs[0] = new Vector2(tile_texture_size * tile_texture_pos.x,
            tile_texture_size * tile_texture_pos.y);
        uvs[1] = new Vector2(tile_texture_size * tile_texture_pos.x,
            tile_texture_size * tile_texture_pos.y + tile_texture_size);
        uvs[2] = new Vector2(tile_texture_size * tile_texture_pos.x + tile_texture_size,
            tile_texture_size * tile_texture_pos.y + tile_texture_size);
        uvs[3] = new Vector2(tile_texture_size * tile_texture_pos.x + tile_texture_size,
            tile_texture_size * tile_texture_pos.y);

        return uvs;
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

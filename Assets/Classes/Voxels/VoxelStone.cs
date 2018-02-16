﻿using System;


[Serializable]
public class VoxelStone : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture//stone only has one texture
        {
            x = 0,
            y = 0
        };
        return tile_texture;
    }
}

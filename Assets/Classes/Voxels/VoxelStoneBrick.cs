using System;


[Serializable]
public class VoxelStoneBrick : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture//stone only has one texture
        {
            x = 0,
            y = 1
        };
        return tile_texture;
    }
}

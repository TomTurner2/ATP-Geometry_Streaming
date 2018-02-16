using System;


[Serializable]
public class VoxelBlueWool : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture//stone only has one texture
        {
            x = 2,
            y = 2
        };
        return tile_texture;
    }
}
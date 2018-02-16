using System;


[Serializable]
public class VoxelWoodPlanks : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture//stone only has one texture
        {
            x = 1,
            y = 1
        };
        return tile_texture;
    }
}

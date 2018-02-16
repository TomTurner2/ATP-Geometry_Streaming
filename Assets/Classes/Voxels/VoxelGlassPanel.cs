using System;


[Serializable]
public class VoxelGlassPanel : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture//stone only has one texture
        {
            x = 1,
            y = 2
        };
        return tile_texture;
    }


    public override bool IsSolid()
    {
        return false;//so a face is created between
    }
}

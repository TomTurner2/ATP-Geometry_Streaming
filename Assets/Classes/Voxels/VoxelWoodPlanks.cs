using System;


[Serializable]
public class VoxelWoodPlanks : Voxel
{
    public override MeshCreator.TileTexture GetTextureCoordsByDirection(MeshCreator.Direction _direction)//base defaults to first tile
    {
        MeshCreator.TileTexture tile_texture = new MeshCreator.TileTexture//stone only has one texture
        {
            x = 1,
            y = 1
        };
        return tile_texture;
    }
}

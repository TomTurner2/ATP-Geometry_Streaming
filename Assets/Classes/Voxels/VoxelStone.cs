using System;


[Serializable]
public class VoxelStone : Voxel
{
    public override MeshCreator.TileTexture GetTextureCoordsByDirection(MeshCreator.Direction _direction)//base defaults to first tile
    {
        MeshCreator.TileTexture tile_texture = new MeshCreator.TileTexture//stone only has one texture
        {
            x = 0,
            y = 0
        };
        return tile_texture;
    }
}

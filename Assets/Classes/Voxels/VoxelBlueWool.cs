using System;


[Serializable]
public class VoxelBlueWool : Voxel
{
    public override MeshCreator.TileTexture GetTextureCoordsByDirection(MeshCreator.Direction _direction)//base defaults to first tile
    {
        MeshCreator.TileTexture tile_texture = new MeshCreator.TileTexture//stone only has one texture
        {
            x = 2,
            y = 2
        };
        return tile_texture;
    }
}
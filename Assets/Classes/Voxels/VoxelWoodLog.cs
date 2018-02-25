using System;


[Serializable]
public class VoxelWoodLog : Voxel
{
    public override MeshCreator.TileTexture GetTextureCoordsByDirection(MeshCreator.Direction direction)
    {
        MeshCreator.TileTexture tile_texture = new MeshCreator.TileTexture();
        switch (direction)
        {
            case MeshCreator.Direction.TOP:
                tile_texture.x = 2;
                tile_texture.y = 1;
                return tile_texture;
            case MeshCreator.Direction.BOTTOM:
                tile_texture.x = 2;
                tile_texture.y = 1;
                return tile_texture;
        }
        tile_texture.x = 3;
        tile_texture.y = 1;
        return tile_texture;
    }
}
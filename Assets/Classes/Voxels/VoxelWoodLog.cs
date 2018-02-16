using System;


[Serializable]
public class VoxelWoodLog : Voxel
{
    public override TileTexture GetTextureCoordsByDirection(Direction direction)
    {
        TileTexture tile_texture = new TileTexture();
        switch (direction)
        {
            case Direction.TOP:
                tile_texture.x = 2;
                tile_texture.y = 1;
                return tile_texture;
            case Direction.BOTTOM:
                tile_texture.x = 2;
                tile_texture.y = 1;
                return tile_texture;
        }
        tile_texture.x = 3;
        tile_texture.y = 1;
        return tile_texture;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [System.Serializable]
    struct TileInfo
    {
        public uint tile_id;
        public Vector2 uv_coordinate;
    }

    [SerializeField] Vector2 tile_map_grid_size = new Vector2(8, 8);
    [SerializeField] Material tile_map_material = null;
    [SerializeField] List<TileInfo> tiles = new List<TileInfo>();

    public Vector2 GetTileUV(byte _tile_id)
    {
        int id = (int)_tile_id;
        foreach (TileInfo tile in tiles)
        {
            if (tile.tile_id == id)
            {
                return tile.uv_coordinate;
            }
                
        }

        Debug.LogWarning("Tile ID: " + id + " not found!");
        return Vector2.zero;
    }


    public Vector2 GetTextureUnit()
    {
        return new Vector2(1.0f / tile_map_grid_size.x, 1.0f / tile_map_grid_size.y);
    }
}

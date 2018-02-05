using UnityEngine;
using System.Collections;
using SimplexNoise;

[System.Serializable]
public class TerrainGenerator
{
    [Header("Stone Base")]
    [SerializeField] float stone_base_height = -20;
    [SerializeField] float stone_base_noise_scale = 0.05f;
    [SerializeField] float stone_base_noise_height = 4.5f;
    [Space]

    [Header("Stone Mountain")]
    [SerializeField] float stone_mountain_height = 48;
    [SerializeField] float stone_mountain_frequency = 0.02f;
    [SerializeField] float stone_minimum_height = -11;
    [Space]

    [Header("Grass")]
    [SerializeField] float grass_base_height = 1;
    [SerializeField] float grass_noise_scale = 0.04f;
    [SerializeField] float grass_noise_height = 3;


    public Chunk GenerateChunk(Chunk _chunk)
    {
        for (int x = _chunk.voxel_world_position.x; x < _chunk.voxel_world_position.x + Chunk.chunk_size; x++)
        {
            for (int z = _chunk.voxel_world_position.z; z < _chunk.voxel_world_position.z + Chunk.chunk_size; z++)
            {
                _chunk = GenerateChunkColumn(_chunk, x, z);
            }
        }
        return _chunk;
    }


    public Chunk GenerateChunkColumn(Chunk _chunk, int _x, int _z)
    {
        int stone_height = GenerateStone(_x, _z);
        int grass_height = GenerateGrass(_x, _z, stone_height);


        for (int y = _chunk.voxel_world_position.y; y < _chunk.voxel_world_position.y + Chunk.chunk_size; ++y)
        {
            SetVoxelType(_chunk, _x, y, _z, stone_height, grass_height);//determine voxel based on height
        }
        return _chunk;
    }


    private void SetVoxelType(Chunk _chunk,int _x, int _y, int _z, int _stone_height, int _grass_height)
    {
        if (_y <= _stone_height)
        {
            _chunk.SetVoxel(_x - _chunk.voxel_world_position.x, _y - _chunk.voxel_world_position.y, _z - _chunk.voxel_world_position.z, new VoxelStone());
        }
        else if (_y <= _grass_height)
        {
            _chunk.SetVoxel(_x - _chunk.voxel_world_position.x, _y - _chunk.voxel_world_position.y, _z - _chunk.voxel_world_position.z, new VoxelGrass());
        }
        else
        {
            _chunk.SetVoxel(_x - _chunk.voxel_world_position.x, _y - _chunk.voxel_world_position.y, _z - _chunk.voxel_world_position.z, new VoxelAir());
        }
    }


    private int GenerateGrass(int _x, int _z, int _stone_height)
    {
        int grass_height = _stone_height + Mathf.FloorToInt(grass_base_height);
        grass_height += GetNoise(_x, 100, _z, grass_noise_scale, Mathf.FloorToInt(grass_noise_height));
        return grass_height;
    }


    private int GenerateStone(int _x, int _z)
    {
        int stone_height = Mathf.FloorToInt(stone_base_height);//set stone base
        stone_height += GetNoise(_x, 0, _z, stone_mountain_frequency, Mathf.FloorToInt(stone_mountain_height));//calculate mountain noise

        if (stone_height < stone_minimum_height)
            stone_height = Mathf.FloorToInt(stone_minimum_height);//make sure it meets min height

        stone_height += GetNoise(_x, 0, _z, stone_base_noise_scale, Mathf.FloorToInt(stone_base_noise_height));//add additional noise
        return stone_height;
    }


    public static int GetNoise(int _x, int _y, int _z, float _scale, int _max)
    {
        return Mathf.FloorToInt((Noise.Generate(_x * _scale, _y * _scale, _z * _scale) + 1f) * (_max / 2f));
    }
}
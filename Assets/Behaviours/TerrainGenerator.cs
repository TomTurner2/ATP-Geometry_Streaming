using UnityEngine;
using System.Collections;
using SimplexNoise;


public class TerrainGenerator
{

    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f;
    float stoneBaseNoiseHeight = 4;
    float stoneMountainHeight = 48;
    float stoneMountainFrequency = 0.008f;
    float stoneMinHeight = -12;
    float dirtBaseHeight = 1;
    float dirtNoise = 0.04f;
    float dirtNoiseHeight = 3;


    public Chunk ChunkGen(Chunk chunk)
    {
        for (int x = chunk.voxel_world_position.x; x < chunk.voxel_world_position.x + Chunk.chunk_size; x++)
        {
            for (int z = chunk.voxel_world_position.z; z < chunk.voxel_world_position.z + Chunk.chunk_size; z++)
            {
                chunk = ChunkColumnGen(chunk, x, z);
            }
        }
        return chunk;
    }


    public Chunk ChunkColumnGen(Chunk chunk, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));
        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);
        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));
        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));
        for (int y = chunk.voxel_world_position.y; y < chunk.voxel_world_position.y + Chunk.chunk_size; y++)
        {
            if (y <= stoneHeight)
            {
                chunk.SetVoxel(x - chunk.voxel_world_position.x, y - chunk.voxel_world_position.y, z - chunk.voxel_world_position.z, new Voxel());
            }
            else if (y <= dirtHeight)
            {
                chunk.SetVoxel(x - chunk.voxel_world_position.x, y - chunk.voxel_world_position.y, z - chunk.voxel_world_position.z, new VoxelGrass());
            }
            else
            {
                chunk.SetVoxel(x - chunk.voxel_world_position.x, y - chunk.voxel_world_position.y, z - chunk.voxel_world_position.z, new VoxelAir());
            }
        }
        return chunk;
    }


    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]//make this class saveable
public class ChunkSave
{
    public Dictionary<int, Voxel> voxels = new Dictionary<int, Voxel>();//voxels stored in dictionary by coordinates

    public ChunkSave(Chunk _chunk)
    {
        for (int x = 0; x < Chunk.chunk_size; ++x)
        {
            for (int y = 0; y < Chunk.chunk_size; ++y)
            {
                for (int z = 0; z < Chunk.chunk_size; ++z)
                {
                    int index = Chunk.GetIndex(x, y, z);
                    if (!_chunk.voxels[index].edited)//if the voxel wasn't edited, ignore it
                        continue;

                    voxels.Add(index, _chunk.voxels[index]);//add edited voxel
                }
            }
        }
    }
}

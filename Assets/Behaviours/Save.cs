using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class Save
{
    public Dictionary<intVector3, Voxel> blocks = new Dictionary<intVector3, Voxel>();

    public Save(Chunk chunk)
    {
        for (int x = 0; x < Chunk.chunk_size; x++)
        {
            for (int y = 0; y < Chunk.chunk_size; y++)
            {
                for (int z = 0; z < Chunk.chunk_size; z++)
                {
                    if (!chunk.voxels[x, y, z].edited)
                        continue;

                    intVector3 position = new intVector3(x, y, z);
                    blocks.Add(position, chunk.voxels[x, y, z]);
                }
            }
        }
    }
}

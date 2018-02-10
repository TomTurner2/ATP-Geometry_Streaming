using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]//make this class saveable
public class ChunkSave
{
    public Dictionary<intVector3, Voxel> voxels = new Dictionary<intVector3, Voxel>();//voxels stored in dictionary by coordinates

    public ChunkSave(Chunk _chunk)
    {
        for (int x = 0; x < Chunk.chunk_size; ++x)
        {
            for (int y = 0; y < Chunk.chunk_size; ++y)
            {
                for (int z = 0; z < Chunk.chunk_size; ++z)
                {
                    if (!_chunk.voxels[x, y, z].edited)//if the voxel wasn't edited, ignore it
                        continue;

                    voxels.Add(new intVector3(x, y, z), _chunk.voxels[x, y, z]);//add edited voxel
                }
            }
        }
    }
}

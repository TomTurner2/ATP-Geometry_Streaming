using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkStreamer : MonoBehaviour
{
    [SerializeField] uint max_chunks = 32;
    [SerializeField] uintVector3 chunk_size = new uintVector3(16, 32, 256);

    [SerializeField] Transform chunk_load_target;
    [SerializeField] WorldGenerator world_generator;
    [SerializeField] VoxelMeshGenerator voxel_mesh_generator;

    List<Chunk> loaded_chunks = new List<Chunk>();
    List<Chunk> edited_chunks = new List<Chunk>();


	void Start ()
    {
		
	}
	

	void Update ()
    {
		
	}


    void LoadChunk()
    {
        
    }
}

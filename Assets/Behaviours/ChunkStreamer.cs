using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ChunkStreamer : MonoBehaviour
{
    [SerializeField] VoxelWorld voxel_world;
    [SerializeField] uint unload_distance = 256;
    [SerializeField] float chunk_unload_delay = 5;
    [SerializeField] uint max_chunk_y = 8;
    [SerializeField] uint max_chunk_radius = 8;
    [SerializeField] uint max_chunks_built_per_frame = 2;
    
    private List<intVector3> update_list = new List<intVector3>();
    private List<intVector3> load_list = new List<intVector3>();
    private float unload_timer = 0;

    private static List<intVector3> chunk_positions = null;
    private static List<ChunkStreamer> active_chunk_streamers = new List<ChunkStreamer>();


    private void Start()
    {
        if (chunk_positions == null)
            GenerateStaticChunkPositions();

        active_chunk_streamers.Add(this);
    }


    void GenerateStaticChunkPositions()
    {
        chunk_positions = new List<intVector3>();
        int chunk_grid_radius = (int)max_chunk_radius;

        for (int x = -chunk_grid_radius; x < chunk_grid_radius; ++x)//create all possible chunk positions
        {
            for (int z = -chunk_grid_radius; z < chunk_grid_radius; ++z)
            {
                chunk_positions.Add(new intVector3(x, 0, z));
            }
        }

        chunk_positions = chunk_positions.OrderBy(pos => Vector3.Distance(intVector3.Zero, pos)).ToList();
    }


    void Update()
    {
        if (UnloadChunks())//if unloading chunks don't try to load more this frame
            return;

        FindChunksToLoad();
        LoadAndUpdateChunks();
    }


    void FindChunksToLoad()
    {
        intVector3 player_position = VoxelWorld.PositionToWorldPosition(transform.position);

        if (update_list.Count != 0)//if there are chunks to build don't load more
            return;

        for (int i = 0; i < chunk_positions.Count; ++i)//for every pre calculated chunk position
        {
            //calculate chunk position
            intVector3 chunk_pos = new intVector3(chunk_positions[i].x * Chunk.chunk_size + player_position.x, 0,
                chunk_positions[i].z * Chunk.chunk_size + player_position.z);//offset array pos to players pos then convert to chunk pos

                Chunk chunk = voxel_world.GetChunk(chunk_pos.x, chunk_pos.y, chunk_pos.z);//try find chunk at position        
            if (ChunkScheduled(chunk, chunk_pos))//if chunk is already loaded or scheduled to build, skip it
                continue;
       
            AddChunkPositionsToLoadLists(chunk_pos);
            return;
        }
    }


    void AddChunkPositionsToLoadLists(intVector3 _chunk_pos)
    {
        int column_height = Mathf.FloorToInt(max_chunk_y * 0.5f);

        for (int y = -column_height; y < column_height; ++y)//load a column of chunks in this position
        {
            for (int x = _chunk_pos.x - Chunk.chunk_size; x <= _chunk_pos.x + Chunk.chunk_size; x += Chunk.chunk_size)//add adjacent chunks as well so they get updated
            {
                for (int z = _chunk_pos.z - Chunk.chunk_size; z <= _chunk_pos.z + Chunk.chunk_size; z += Chunk.chunk_size)
                {
                    load_list.Add(new intVector3(x, y * Chunk.chunk_size, z));
                }
            }
            update_list.Add(new intVector3(_chunk_pos.x, y * Chunk.chunk_size, _chunk_pos.z));
        }
    }


    private bool ChunkScheduled(Chunk _chunk, intVector3 _chunk_pos)
    {
        return _chunk != null && (_chunk.rendered || update_list.Contains(_chunk_pos));
    }


    void LoadAndUpdateChunks()
    {
        if (load_list.Count != 0)
        {
            for (int i = 0; i < load_list.Count && i < max_chunks_built_per_frame; ++i)
            {
                LoadChunk(load_list[0]);
                load_list.RemoveAt(0);//index 0 as entries are removed
            }
            return;
        }

        if (update_list.Count == 0)
            return;

        for (int i = 0; i < update_list.Count; ++i)
        {
            Chunk chunk = voxel_world.GetChunk(update_list[0].x,
                update_list[0].y, update_list[0].z);//try and get chunk

            if (chunk != null)
                chunk.edited = true;//set it to update its mesh

            update_list.RemoveAt(0);
        }
    }


    void LoadChunk(intVector3 _position)
    {
        if (voxel_world.GetChunk(_position.x, _position.y, _position.z) == null)
            voxel_world.CreateChunk(_position.x, _position.y, _position.z);
    }


    bool UnloadChunks()
    {
        if (unload_timer >= Mathf.Max(0, chunk_unload_delay))//unload after delay (min delay 0)
        {
            List<intVector3> chunks_to_unload = new List<intVector3>();

            foreach (KeyValuePair<intVector3, Chunk> chunk in voxel_world.chunks)
            {
                TryScheduleForUnload(chunk, chunks_to_unload);//determine if chunk needs unloading
            }

            chunks_to_unload.ForEach(chunk => voxel_world.DestroyChunk(chunk.x, chunk.y, chunk.z));//unload these chunks

            unload_timer = 0;
            return true;
        }

        unload_timer += Time.deltaTime;
        return false;
    }


    void TryScheduleForUnload(KeyValuePair<intVector3, Chunk> _chunk, List<intVector3> _chunks_to_unload)
    {
        bool unload = true;
        foreach (ChunkStreamer streamer in active_chunk_streamers)
        {
            if (!streamer.isActiveAndEnabled)
                continue;

            float distance = (new Vector3(_chunk.Value.voxel_world_position.x, 0, _chunk.Value.voxel_world_position.z) -
                            new Vector3(streamer.transform.position.x, 0, streamer.transform.position.z)).sqrMagnitude;//avoid square root

            if (distance < unload_distance * unload_distance)
                unload = false;
        }

        if (unload)//if out of range of all active streamers schedule the chunk
            _chunks_to_unload.Add(_chunk.Key);
    }


    private void OnDestroy()
    {
        active_chunk_streamers.Remove(this);
    }
}

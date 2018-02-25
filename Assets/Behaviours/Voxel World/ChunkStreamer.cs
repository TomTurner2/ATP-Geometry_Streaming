using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class ChunkStreamer : MonoBehaviour
{
    [SerializeField] VoxelWorld voxel_world;
    [SerializeField] uint unload_distance = 256;
    [SerializeField] float chunk_unload_delay = 5;
    [SerializeField] uint max_chunk_y = 8;
    [SerializeField] uint max_chunk_radius = 8;
    [SerializeField] uint max_chunks_built_per_frame = 2;
    [SerializeField] bool multithreading = false;
    [SerializeField] bool object_pooling = true;
    
    private List<intVector3> update_list = new List<intVector3>();
    private List<intVector3> load_list = new List<intVector3>();
    private float unload_timer = 0;

    private ThreadManager thread_manager = new ThreadManager();
    intVector3 current_world_position;
    List<Vector3> streamer_positions = new List<Vector3>();

    static List<Chunk> available_chunks = new List<Chunk>(); 
    static List<Chunk> used_chunks = new List<Chunk>();

    private static List<intVector3> chunk_positions = null;
    private static List<ChunkStreamer> active_chunk_streamers = new List<ChunkStreamer>();


    private void Start()
    {
        if (chunk_positions == null)
            GenerateStaticChunkPositions();

        active_chunk_streamers.Add(this);
    }


    private void OnDisable()
    {
        active_chunk_streamers.Remove(this);
    }


    private void OnEnable()
    {
        if (!active_chunk_streamers.Contains(this))
            active_chunk_streamers.Add(this);
    }


    private void GenerateStaticChunkPositions()
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

        chunk_positions = chunk_positions.OrderBy(pos => Vector3.Distance(intVector3.Zero, pos)).ToList();//order by closeness to centre 0, 0
    }


    public  Chunk GetChunkFromPool()
    {
        lock (available_chunks)
        {
            if (available_chunks.Count != 0)
            {
                Chunk chunk = available_chunks[0];
                used_chunks.Add(chunk);
                available_chunks.RemoveAt(0);
                return chunk;
            }
            else
            {
                Chunk chunk = voxel_world.CreateEmptyChunk();
                used_chunks.Add(chunk);
                return chunk;
            }
        }
    }


    public void ReleaseChunk(Chunk _chunk)
    {
        available_chunks.Add(_chunk);
        used_chunks.Add(voxel_world.RemoveChunk(_chunk));
    }


    private void Update()
    {   
        current_world_position = VoxelWorld.PositionToWorldPosition(transform.position);

        if (multithreading)
        {
            ThreadedUpdate();
            return;
        }

        RegularUpdate();
    }


    void ThreadedUpdate()
    {
        current_world_position = transform.position;
        streamer_positions = GetActiveStreamerPositions();

        thread_manager.StartThreadedJob(UnloadChunksThreadSafe);
        thread_manager.StartThreadedJob(()=>
        {
            FindChunksToLoad();
            LoadAndUpdateThreadSafe();
        });
        thread_manager.Update();// bottleneck
    }


    void LoadAndUpdateThreadSafe()
    {
        if (load_list.Count != 0)
        {
            for (int i = 0; i < load_list.Count; ++i)
            {
                intVector3 temp = load_list[0];
                thread_manager.QueueForMainThread(()=>
                {
                    if (object_pooling)
                    {
                        ReuseChunk(voxel_world, temp);
                    }
                    else
                    {
                        LoadChunk(voxel_world, temp);
                    }
                });
               
                load_list.RemoveAt(0);//index 0 as entries are removed
            }
        }

        if (update_list.Count == 0)
            return;

        for (int i = 0; i < update_list.Count; ++i)
        {
            intVector3 temp = update_list[0];
            thread_manager.QueueForMainThread(() =>
            {
                Chunk chunk = voxel_world.GetChunkUnFloored(temp);//try and get chunk

                if (chunk != null)
                    chunk.edited = true;//set it to update its mesh
            });

            update_list.RemoveAt(0);
        }
    }


    void RegularUpdate()
    {
        if (UnloadChunks())//if unloading chunks don't try to load more this frame
            return;

        FindChunksToLoad();
        LoadAndUpdateChunks();
    }


    private void FindChunksToLoad()
    {
        if (update_list.Count != 0)//if there are chunks to build don't load more
            return;

        for (int i = 0; i < chunk_positions.Count; ++i)//for every pre calculated chunk position
        {
            //calculate chunk position
            intVector3 chunk_pos = new intVector3(chunk_positions[i].x * Chunk.chunk_size + current_world_position.x, 0,
                chunk_positions[i].z * Chunk.chunk_size + current_world_position.z);//offset array pos to players pos then convert to chunk pos
   
            Chunk chunk = voxel_world.GetChunk(chunk_pos.x, chunk_pos.y, chunk_pos.z);//try find chunk at position        
            if (ChunkScheduled(chunk, chunk_pos))//if chunk is already loaded or scheduled to build, skip it
                continue;

            AddChunkPositionsToLoadLists(chunk_pos);

            return;
        }
        
    }


    private void AddChunkPositionsToLoadLists(intVector3 _chunk_pos)
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


    private void LoadAndUpdateChunks()
    {
        if (load_list.Count != 0)
        {
            for (int i = 0; i < load_list.Count && (i < max_chunks_built_per_frame); ++i)
            {
                if (object_pooling)
                {
                    ReuseChunk(voxel_world, load_list[0]);
                }
                else
                {
                    LoadChunk(voxel_world, load_list[0]);      
                }

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


    private void ReuseChunk(VoxelWorld _voxel_world, intVector3 _position)
    {
        if (_voxel_world.GetChunk(_position.x, _position.y, _position.z) != null)
            return;

        _voxel_world.ReuseChunk(GetChunkFromPool(), _position);//reuse chunk if available
    }


    private void LoadChunk(VoxelWorld _voxel_world, intVector3 _position)
    {
        if (_voxel_world.GetChunk(_position.x, _position.y, _position.z) == null)
            _voxel_world.CreateChunk(_position.x, _position.y, _position.z);
    }


    private bool UnloadChunks()
    {
        if (unload_timer >= Mathf.Max(0, chunk_unload_delay))//unload after delay (min delay 0)
        {
            TryUnloadChunks();
            unload_timer = 0;
            return true;
        }

        unload_timer += Time.deltaTime;
        return false;
    }


    private void TryUnloadChunks()
    {
        List<intVector3> chunks_to_unload = new List<intVector3>();

        foreach (KeyValuePair<intVector3, Chunk> chunk in voxel_world.chunks)
        {
            TryScheduleForUnload(chunk, chunks_to_unload, GetActiveStreamerPositions());//determine if chunk needs unloading
        }

        if (object_pooling)
        {
            foreach (intVector3 chunk_pos in chunks_to_unload)
            {
                Chunk chunk = voxel_world.GetChunk(chunk_pos.x, chunk_pos.y, chunk_pos.z);
                ReleaseChunk(chunk);
            }
        }
        else
        {
            chunks_to_unload.ForEach(chunk => voxel_world.DestroyChunk(chunk.x, chunk.y, chunk.z));//unload these chunks
        }

    }


    List<Vector3> GetActiveStreamerPositions()
    {
        List<Vector3> positions = active_chunk_streamers.Select(streamer => streamer.transform.position).ToList();
        return positions;
    }


    private void UnloadChunksThreadSafe()
    {
        List<intVector3> chunks_to_unload = new List<intVector3>();

        foreach (KeyValuePair<intVector3, Chunk> chunk in voxel_world.chunks)
        {
            TryScheduleForUnload(chunk, chunks_to_unload, streamer_positions);//determine if chunk needs unloading
        }

        chunks_to_unload.ForEach(chunk => thread_manager.QueueForMainThread(() =>
        {
            voxel_world.DestroyChunk(chunk.x, chunk.y, chunk.z);
        }));//unload these chunks on main thread
    }


    private void TryScheduleForUnload(KeyValuePair<intVector3, Chunk> _chunk, List<intVector3> _chunks_to_unload, List<Vector3> _streamer_positions)
    {
        bool unload = true;
        foreach (Vector3 pos in _streamer_positions)
        {
            float distance = (new Vector3(_chunk.Value.voxel_world_position.x, 0, _chunk.Value.voxel_world_position.z) -
                            new Vector3(pos.x, 0, pos.z)).sqrMagnitude;//avoid square root

            if (distance < unload_distance * unload_distance)
                unload = false;
        }

        if (unload)//if out of range of all active streamers schedule the chunk
            _chunks_to_unload.Add(_chunk.Key);
    }


    private void OnDestroy()
    {
        active_chunk_streamers.Remove(this);
        thread_manager.OnDestroy();
    }
}

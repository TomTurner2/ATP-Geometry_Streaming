using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MeshGenerationType
{
    NAIVE_CUBES,
    CUBES
};


public class VoxelWorld : MonoBehaviour
{
    [HideInInspector] public string save_name = "World";
    [HideInInspector] public Dictionary<intVector3, Chunk> chunks = new Dictionary<intVector3, Chunk>();

    public GameObject chunk_prefab;
    public MeshGenerationType mesh_generation_type = MeshGenerationType.NAIVE_CUBES;
    [Space]

    [SerializeField] TerrainGenerator terrain_generator = new TerrainGenerator();


    private void Start()
    {
        save_name = GameInfo.WorldName;//get chosen world name
    }


    public void CreateChunk(int _x, int _y, int _z)
    {      
        intVector3 voxel_world_position = new intVector3(_x, _y, _z);//the coordinates of chunk in the world

        if (chunks.ContainsKey(voxel_world_position))
            return;

        GameObject chunk_object = Instantiate(chunk_prefab, new Vector3(voxel_world_position.x,
            voxel_world_position.y, voxel_world_position.z),Quaternion.Euler(Vector3.zero));//create chunk

        //set chunk parameters
        Chunk chunk = chunk_object.GetComponent<Chunk>();
        chunk.voxel_world_position = voxel_world_position;
        chunk.voxel_world = this;
        chunk.transform.parent = transform;
        chunk.name = "World Chunk";
        chunk.gameObject.isStatic = true;

        chunks.Add(voxel_world_position, chunk);//store chunk in dictionary by its position
        chunk = terrain_generator.GenerateChunk(chunk);

        chunk.SetVoxelsUnEdited();
        VoxelWorldSaver.Load(chunk);
    }


    public Chunk CreateEmptyChunk()
    {
        GameObject chunk_object = Instantiate(chunk_prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));//create chunk

        //set chunk parameters
        Chunk chunk = chunk_object.GetComponent<Chunk>();
        chunk.voxel_world_position = Vector3.zero;
        chunk.voxel_world = this;
        chunk.transform.parent = transform;
        chunk.name = "World Chunk";
        chunk.gameObject.isStatic = true;

        return chunk;
    }


    public void ReuseChunk(Chunk _chunk, intVector3 _position)
    {
       // intVector3 voxel_world_position = new intVector3(_x, _y, _z);//the coordinates of chunk in the world

        if (_chunk == null)
            CreateChunk(_position.x, _position.y, _position.z);

        _chunk.transform.position = _position;
        _chunk.voxel_world_position = _position;

        _chunk.gameObject.SetActive(true);

        _chunk.voxel_world = this;
        _chunk.transform.parent = transform;
        _chunk.name = "World Chunk";
        _chunk.gameObject.isStatic = true;

        chunks.Add(_position, _chunk);//store chunk in dictionary by its position
        _chunk = terrain_generator.GenerateChunk(_chunk);

        _chunk.SetVoxelsUnEdited();
        VoxelWorldSaver.Load(_chunk);      
    }


    public Chunk RemoveChunk(Chunk _chunk)
    {
        Chunk chunk = null;
        if (!chunks.TryGetValue(new intVector3(_chunk.voxel_world_position.x, _chunk.voxel_world_position.y, _chunk.voxel_world_position.z), out chunk))//if not in dictionary leave
            return null;

        VoxelWorldSaver.SaveChunk(chunk);//save chunk
        chunks.Remove(new intVector3(_chunk.voxel_world_position.x, _chunk.voxel_world_position.y, _chunk.voxel_world_position.z));//remove entry from dictionary

        chunk.gameObject.SetActive(false);
        chunk.ClearMeshData();
        chunk.edited = false;
        chunk.rendered = false;
        return chunk;
    }


    public void DestroyChunk(int _x, int _y, int _z)
    {
        Chunk chunk = null;
        if (!chunks.TryGetValue(new intVector3(_x, _y, _z), out chunk))//if not in dictionary leave
            return;

        VoxelWorldSaver.SaveChunk(chunk);//save chunk
        Destroy(chunk.gameObject);//destroy chunk
        chunks.Remove(new intVector3(_x, _y, _z));//remove entry from dictionary
    }


    public Chunk GetChunk(int _x, int _y, int _z)
    {
        intVector3 position = new intVector3();
        float chunk_size = Chunk.chunk_size;//convert to float for division

        position.x = Mathf.FloorToInt(_x / chunk_size) * Chunk.chunk_size;
        position.y = Mathf.FloorToInt(_y / chunk_size) * Chunk.chunk_size;
        position.z = Mathf.FloorToInt(_z / chunk_size) * Chunk.chunk_size;

        Chunk chunk = null;
        chunks.TryGetValue(position, out chunk);

        return chunk;
    }


    public Chunk GetChunkUnFloored(intVector3 _position)
    {
        Chunk chunk = null;
        chunks.TryGetValue(_position, out chunk);

        return chunk;
    }


    public Voxel GetVoxel(int _x, int _y, int _z)
    {
        Chunk chunk = GetChunk(_x, _y, _z);

        if (chunk == null)
            return new VoxelAir();

        Voxel voxel = chunk.GetVoxel(_x - chunk.voxel_world_position.x, _y - chunk.voxel_world_position.y,
            _z - chunk.voxel_world_position.z);

        return voxel;
    }


    public void SetVoxel(int _x, int _y, int _z, Voxel _voxel)
    {
        Chunk chunk = GetChunk(_x, _y, _z);

        if (chunk == null)
            return;

        intVector3 voxel_chunk_pos = new intVector3(_x - chunk.voxel_world_position.x,
            _y - chunk.voxel_world_position.y, _z - chunk.voxel_world_position.z);

        chunk.SetVoxel(voxel_chunk_pos.x, voxel_chunk_pos.y, voxel_chunk_pos.z, _voxel);
        chunk.edited = true;

        //update neighbouring chunks
        SetNeighbourEditedIfEqual(voxel_chunk_pos.x, 0, new intVector3(_x - 1, _y, _z));//if offset x - 1 less than zero (in adjacent chunk) the neighbouring chunk should update
        SetNeighbourEditedIfEqual(voxel_chunk_pos.x, Chunk.chunk_size - 1, new intVector3(_x + 1, _y, _z));
        SetNeighbourEditedIfEqual(voxel_chunk_pos.y, 0, new intVector3(_x, _y - 1, _z));
        SetNeighbourEditedIfEqual(voxel_chunk_pos.y, Chunk.chunk_size - 1, new intVector3(_x, _y + 1, _z));
        SetNeighbourEditedIfEqual(voxel_chunk_pos.z, 0, new intVector3(_x, _y, _z - 1));
        SetNeighbourEditedIfEqual(voxel_chunk_pos.z, Chunk.chunk_size - 1, new intVector3(_x, _y, _z + 1));
    }


    void SetNeighbourEditedIfEqual(int _voxel_chunk_position, int _check_value, intVector3 _position)
    {
        if (_voxel_chunk_position != _check_value)
            return;

        Chunk chunk = GetChunk(_position.x, _position.y, _position.z);
        if (chunk != null)
            chunk.edited = true;//set chunk as edited so it updates
    }


    public static intVector3 PositionToWorldPosition(Vector3 _position)
    {
        return new intVector3(Mathf.FloorToInt(_position.x / Chunk.chunk_size) * Chunk.chunk_size,
            Mathf.FloorToInt(_position.y / Chunk.chunk_size) * Chunk.chunk_size,
            Mathf.FloorToInt(_position.z / Chunk.chunk_size) * Chunk.chunk_size);//snaps position to int
    }


    public void SaveWorld()
    {
        foreach (KeyValuePair<intVector3, Chunk> chunk in chunks)
        {
            VoxelWorldSaver.SaveChunk(chunk.Value);//save chunk
        }
    }


    private void OnDestroy()
    {
        SaveWorld();
    }
}

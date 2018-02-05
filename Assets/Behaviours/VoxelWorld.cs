using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MeshGenerationType
{
    NAIVE_CUBES,
    CUBES, 
    MARCHING_CUBES
};


public class VoxelWorld : MonoBehaviour
{
    [HideInInspector] public string save_name = "World";
    [HideInInspector] public Dictionary<intVector3, Chunk> chunks = new Dictionary<intVector3, Chunk>();

    public GameObject chunk_prefab;
    public MeshGenerationType mesh_generation_type = MeshGenerationType.NAIVE_CUBES;
    [Space]

    [SerializeField] TerrainGenerator terrain_generator = new TerrainGenerator();


    public void CreateChunk(int _x, int _y, int _z)
    {
        //the coordinates of chunk in the world
        intVector3 voxel_world_position = new intVector3(_x, _y, _z);

        GameObject chunk_object = Instantiate(chunk_prefab, new Vector3(voxel_world_position.x,
            voxel_world_position.y, voxel_world_position.z),Quaternion.Euler(Vector3.zero));

        Chunk chunk = chunk_object.GetComponent<Chunk>();
        chunk.voxel_world_position = voxel_world_position;
        chunk.voxel_world = this;
        chunk.transform.parent = transform;
        chunk.name = "World Chunk";
        chunk.gameObject.isStatic = true;

        chunks.Add(voxel_world_position, chunk);//store chunk in dictionary by its position
        chunk = terrain_generator.GenerateChunk(chunk);

        chunk.SetVoxelsUnEdited();
        Serialization.Load(chunk);
    }


    public void DestroyChunk(int _x, int _y, int _z)
    {
        Chunk chunk = null;
        if (!chunks.TryGetValue(new intVector3(_x, _y, _z), out chunk))//if not in dictionary leave
            return;

        Serialization.SaveChunk(chunk);//save chunk
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


    public Voxel GetBlock(int _x, int _y, int _z)
    {
        Chunk chunk = GetChunk(_x, _y, _z);

        if (chunk == null)
            return new VoxelAir();

        Voxel voxel = chunk.GetVoxel(_x - chunk.voxel_world_position.x, _y - chunk.voxel_world_position.y,
            _z - chunk.voxel_world_position.z);

        return voxel;
    }


    public void SetBlock(int _x, int _y, int _z, Voxel _voxel)
    {
        Chunk chunk = GetChunk(_x, _y, _z);

        if (chunk == null)
            return;

        intVector3 voxel_chunk_pos = new intVector3(_x - chunk.voxel_world_position.x, _y - chunk.voxel_world_position.y, _z - chunk.voxel_world_position.z);

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
}

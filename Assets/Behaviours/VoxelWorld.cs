using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    public string worldName = "world";
    public Dictionary<intVector3, Chunk> chunks = new Dictionary<intVector3, Chunk>();
    public GameObject chunkPrefab;
    //public int newChunkX;
    //public int newChunkY;
    //public int newChunkZ;

    //public bool genChunk;

    // Use this for initialization
    void Start()
    {
        for (int x = -4; x < 4; x++)
        {
            for (int y = -1; y < 0; y++)
            {
                for (int z = -4; z < 4; z++)
                {
                    CreateChunk(x * 16, y * 16, z * 16);
                }
            }
        }
    }


    void Update ()
    {
        //if (genChunk)
        //{
        //    genChunk = false;
        //    WorldPos chunkPos = new WorldPos(newChunkX, newChunkY, newChunkZ);
        //    Chunk chunk = null;

        //    if (chunks.TryGetValue(chunkPos, out chunk))
        //    {
        //        DestroyChunk(chunkPos.x, chunkPos.y, chunkPos.z);
        //    }
        //    else
        //    {
        //        CreateChunk(chunkPos.x, chunkPos.y, chunkPos.z);
        //    }
        //}
    }


    public void CreateChunk(int _x, int _y, int _z)
    {
        //the coordinates of chunk in the world
        intVector3 voxel_world_position = new intVector3(_x, _y, _z);

        GameObject chunk_object = Instantiate(chunkPrefab, new Vector3(voxel_world_position.x,
            voxel_world_position.y, voxel_world_position.z),Quaternion.Euler(Vector3.zero));

        Chunk chunk = chunk_object.GetComponent<Chunk>();
        chunk.voxel_world_position = voxel_world_position;
        chunk.voxel_voxel_world = this;
        chunk.transform.parent = transform;
        chunk.name = "World Chunk";

        chunks.Add(voxel_world_position, chunk);//store chunk in dictionary by its position


        for (int xi = 0; xi < Chunk.chunk_size; xi++)
        {
            for (int yi = 0; yi < Chunk.chunk_size; yi++)
            {
                for (int zi = 0; zi < Chunk.chunk_size; zi++)
                {
                    if (yi <= 7)
                    {
                        if (yi <= 6)
                        {
                            SetBlock(_x + xi, _y + yi, _z + zi, new VoxelStone());
                        }
                        else
                        {
                            SetBlock(_x + xi, _y + yi, _z + zi, new VoxelGrass());
                        }
                        
                    }
                    else
                    {
                        SetBlock(_x + xi, _y + yi, _z + zi, new VoxelAir());
                    }
                }
            }
        }


        //test
        //var terrainGen = new TerrainGenerator();
        //newChunk = terrainGen.ChunkGen(newChunk);

        chunk.SetVoxelsUnEdited();
        Serialization.Load(chunk);
    }


    public void DestroyChunk(int x, int y, int z)
    {
        Chunk chunk = null;
        if (!chunks.TryGetValue(new intVector3(x, y, z), out chunk))//if not in dictionary leave
            return;

        Serialization.SaveChunk(chunk);//if chunk exists save it
        UnityEngine.Object.Destroy(chunk.gameObject);//destroy chunk
        chunks.Remove(new intVector3(x, y, z));//remove entry from dictionary
    }


    public Chunk GetChunk(int x, int y, int z)
    {
        intVector3 position = new intVector3();
        float multiple = Chunk.chunk_size;
        position.x = Mathf.FloorToInt(x / multiple) * Chunk.chunk_size;
        position.y = Mathf.FloorToInt(y / multiple) * Chunk.chunk_size;
        position.z = Mathf.FloorToInt(z / multiple) * Chunk.chunk_size;

        Chunk containerChunk = null;
        chunks.TryGetValue(position, out containerChunk);

        return containerChunk;
    }


    public Voxel GetBlock(int x, int y, int z)
    {
        Chunk containerChunk = GetChunk(x, y, z);
        if (containerChunk != null)
        {
            Voxel voxel = containerChunk.GetVoxel(
                x - containerChunk.voxel_world_position.x,
                y - containerChunk.voxel_world_position.y,
                z - containerChunk.voxel_world_position.z);

            return voxel;
        }
        else
        {
            return new VoxelAir();
        }

    }


    public void SetBlock(int x, int y, int z, Voxel voxel)
    {
        Chunk chunk = GetChunk(x, y, z);

        if (chunk == null)
            return;

        chunk.SetVoxel(x - chunk.voxel_world_position.x, y - chunk.voxel_world_position.y, z - chunk.voxel_world_position.z, voxel);
        chunk.edited = true;

        //update neighbouring chunks
        UpdateIfEqual(x - chunk.voxel_world_position.x, 0, new intVector3(x - 1, y, z));
        UpdateIfEqual(x - chunk.voxel_world_position.x, Chunk.chunk_size - 1, new intVector3(x + 1, y, z));
        UpdateIfEqual(y - chunk.voxel_world_position.y, 0, new intVector3(x, y - 1, z));
        UpdateIfEqual(y - chunk.voxel_world_position.y, Chunk.chunk_size - 1, new intVector3(x, y + 1, z));
        UpdateIfEqual(z - chunk.voxel_world_position.z, 0, new intVector3(x, y, z - 1));
        UpdateIfEqual(z - chunk.voxel_world_position.z, Chunk.chunk_size - 1, new intVector3(x, y, z + 1));
    }


    void UpdateIfEqual(int value1, int value2, intVector3 position)
    {
        if (value1 == value2)
        {
            Chunk chunk = GetChunk(position.x, position.y, position.z);
            if (chunk != null)
                chunk.edited = true;
        }
    }
}

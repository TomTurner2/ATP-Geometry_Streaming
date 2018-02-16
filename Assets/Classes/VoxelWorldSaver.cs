using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public static class VoxelWorldSaver
{
    public static string save_folder_name = "WorldSaves";


    public static string[] GetAllWorldSaves()
    {
        string path = save_folder_name + "/";
        string[] saves = Directory.GetDirectories(path);

        for (int i =0; i < saves.Length; ++i)
        {
            saves[i] = saves[i].Remove(0, path.Length);//remove directory from string
        }

        return saves;
    }


    public static void DeleteSave(string _world_name)
    {
        string save_location = save_folder_name + "/" + _world_name + "/";

        if (Directory.Exists(save_location))
            Directory.Delete(save_location, true);
    }


    public static string GetSaveLocation(string _world_name)
    {
        string save_location = save_folder_name + "/" + _world_name + "/";

        if (!Directory.Exists(save_location))//if not already existing
        {
            Directory.CreateDirectory(save_location);//create directory
        }

        return save_location;
    }


    public static string GetChunkFileName(intVector3 _chunk_position)
    {
        return _chunk_position.x + "," + _chunk_position.y + "," + _chunk_position.z + ".bin";
    }


    public static void SaveChunk(Chunk _chunk)
    {
        ChunkSave chunk_save = new ChunkSave(_chunk);//create save data from chunk

        if (chunk_save.voxels.Count == 0)//if no edited voxels
            return;//don't bother saving

        string save_file = GetSaveLocation(_chunk.voxel_world.save_name);
        save_file += GetChunkFileName(_chunk.voxel_world_position);//add save name onto file directory

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(save_file, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, chunk_save);//serialise the save object
        stream.Close();
    }


    public static bool Load(Chunk _chunk)
    {
        string save_file = GetSaveLocation(_chunk.voxel_world.save_name);
        save_file += GetChunkFileName(_chunk.voxel_world_position);//determine possible directory of chunk save

        if (!File.Exists(save_file))//if no save don't attempt to load
            return false;

        IFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(save_file, FileMode.Open);//open file
        ChunkSave chunk_save = (ChunkSave)formatter.Deserialize(stream);//unserialise file into chunk save

        foreach (KeyValuePair<intVector3, Voxel> voxel in chunk_save.voxels)
        {
            _chunk.voxels[voxel.Key.x, voxel.Key.y, voxel.Key.z] = voxel.Value;
        }

        stream.Close();//close file stream
        return true;
    }

}

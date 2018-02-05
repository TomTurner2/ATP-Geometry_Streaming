using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
public static class Serialization
{
    public static string saveFolderName = "WorldSaves";

    public static string SaveLocation(string worldName)
    {
        string saveLocation = saveFolderName + "/" + worldName + "/";

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }


    public static string FileName(intVector3 chunkLocation)
    {
        string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
        return fileName;
    }

    public static void SaveChunk(Chunk chunk)
    {
        Save save = new Save(chunk);    //Add these lines
        if (save.blocks.Count == 0)     //to the start
            return;                     //of the function
        string saveFile = SaveLocation(chunk.voxel_world.save_name);
        saveFile += FileName(chunk.voxel_world_position);
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, save);  //And change this to use save instead of chunk.blocks
        stream.Close();
    }


    public static bool Load(Chunk chunk)
    {
        string saveFile = SaveLocation(chunk.voxel_world.save_name);
        saveFile += FileName(chunk.voxel_world_position);

        if (!File.Exists(saveFile))
            return false;

        IFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Open);

        Save save = (Save)formatter.Deserialize(stream);
        foreach (var block in save.blocks)
        {
            chunk.voxels[block.Key.x, block.Key.y, block.Key.z] = block.Value;
        }
        stream.Close();
        return true;
    }



}

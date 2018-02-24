using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] TerrainInteractor interactor = null;

    private Dictionary<KeyCode, Voxel> hot_bar = new Dictionary<KeyCode, Voxel>()//link keys to a block
    {
        { KeyCode.Alpha1, new VoxelStone() },
        { KeyCode.Alpha2, new VoxelGrass() },
        { KeyCode.Alpha3, new VoxelStoneBrick() },
        { KeyCode.Alpha4, new VoxelWoodPlanks() },
        { KeyCode.Alpha5, new VoxelGlassPanel() },
        { KeyCode.Alpha6, new VoxelWoodLog() },
        { KeyCode.Alpha7, new VoxelBlueWool() },
        { KeyCode.Alpha8, new VoxelChair() }
    };


    void Update ()
    {
        if (interactor == null)
            return;

        foreach (KeyValuePair<KeyCode, Voxel> hot_bar_item in hot_bar)
        {
            if (Input.GetKeyDown(hot_bar_item.Key))
                interactor.BlockToPlace = hot_bar_item.Value;
        }
    }
}

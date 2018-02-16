using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInteractor : MonoBehaviour
{
    [SerializeField] int max_edit_distance = 100;
    private Voxel block_to_place = new VoxelStone();


    public Voxel BlockToPlace
    {
        get
        {
            return block_to_place;
        }
        set
        {
            block_to_place = value;
        }
    }


	void Update ()
    {
        if (Cursor.lockState == CursorLockMode.None)//don't do it if interacting with ui
            return;

        DetectPlaceVoxel();
        DetectRemoveVoxel();
    }


    void DetectPlaceVoxel()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, max_edit_distance))
        {
            Terraformer.SetVoxel(hit, block_to_place, true);//place in adjacent
        }
    }


    void DetectRemoveVoxel()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, max_edit_distance))
        {
            Terraformer.SetVoxel(hit, new VoxelAir());//replace with air
        }
    }
}

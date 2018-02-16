using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInteractor : MonoBehaviour
{
    [SerializeField] int max_edit_distance = 100;


	void Update ()
    {
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
            Terraformer.SetVoxel(hit, new VoxelStone(), true);//place in adjacent
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

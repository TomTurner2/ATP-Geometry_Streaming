using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInteractor : MonoBehaviour
{
    [SerializeField] int max_edit_distance = 100;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


	void Update ()
    {
        DetectPlaceVoxel();
        DetectRemoveVoxel();

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        if (Input.GetMouseButton(0))
            Cursor.lockState = CursorLockMode.Locked;
#endif
    }


    void DetectPlaceVoxel()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse1))
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, max_edit_distance))
        {
            Terraformer.SetVoxel(hit, new VoxelStone(), true);//place in adjacent
        }
    }


    void DetectRemoveVoxel()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, max_edit_distance))
        {
            Terraformer.SetVoxel(hit, new VoxelAir());//replace with air
        }
    }
}

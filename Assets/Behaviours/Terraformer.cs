using System;
using UnityEngine;
using System.Collections;


public static class Terraformer
{
    public static intVector3 GetBlockPos(Vector3 _pos)
    {
        intVector3 block_position = new intVector3(
            Mathf.RoundToInt(_pos.x),
            Mathf.RoundToInt(_pos.y),
            Mathf.RoundToInt(_pos.z)
        );

        return block_position;
    }


    public static intVector3 GetBlockPos(RaycastHit _hit, bool _adjacent = false)
    {
        Vector3 pos = new Vector3(
            MoveToVoxelCentre(_hit.point.x, _hit.normal.x, _adjacent),
            MoveToVoxelCentre(_hit.point.y, _hit.normal.y, _adjacent),
            MoveToVoxelCentre(_hit.point.z, _hit.normal.z, _adjacent)
        );

        return GetBlockPos(pos);
    }


    static float MoveToVoxelCentre(float _pos, float _normal, bool _adjacent = false)
    {
        if (Math.Abs(_pos - (int) _pos - 0.5f) > 0.1f && Math.Abs(_pos - (int) _pos - (-0.5f)) > 0.1f)//check if position is not already inside block
            return (float) _pos;

        if (_adjacent)
        {
            _pos += (_normal * 0.5f);
        }
        else
        {
            _pos -= (_normal * 0.5f);
        }

        return (float)_pos;
    }


    public static bool SetBlock(RaycastHit _hit, Voxel _voxel, bool _adjacent = false)
    {
        Chunk chunk = _hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return false;

        intVector3 position = GetBlockPos(_hit, _adjacent);

        chunk.voxel_voxel_world.SetBlock(position.x, position.y, position.z, _voxel);

        return true;
    }


    public static Voxel GetBlock(RaycastHit _hit, bool _adjacent = false)
    {
        Chunk chunk = _hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return null;

        intVector3 position = GetBlockPos(_hit, _adjacent);
        Voxel voxel = chunk.voxel_voxel_world.GetBlock(position.x, position.y, position.z);

        return voxel;
    }

}

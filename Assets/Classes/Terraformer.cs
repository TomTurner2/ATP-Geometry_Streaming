using System;
using UnityEngine;
using System.Collections;


public static class Terraformer
{
    private static intVector3 GetVoxelCentrePosition(RaycastHit _hit, bool _adjacent = false)
    {
        Vector3 pos = new Vector3(MoveToVoxelCentre(_hit.point.x, _hit.normal.x, _adjacent),
            MoveToVoxelCentre(_hit.point.y, _hit.normal.y, _adjacent),
            MoveToVoxelCentre(_hit.point.z, _hit.normal.z, _adjacent));//move hit location inside voxel

        return pos;
    }


    static float MoveToVoxelCentre(float _pos, float _normal, bool _adjacent = false)
    {
        if (AxisPositionIsCentred(_pos))
            return _pos;

        if (_adjacent)
        {
            _pos += (_normal * 0.5f);//move to centre of adjacent voxel
            return _pos;
        }

        _pos -= (_normal * 0.5f);//move to centre of hit voxel
        return _pos;
    }


    private static bool AxisPositionIsCentred(float _pos)
    {
        return (Math.Abs(_pos - (int)_pos - 0.5f) > 0.1f && Math.Abs(_pos - (int)_pos - (-0.5f)) > 0.1f);//check if position is not already inside block
    }


    public static bool SetVoxel(RaycastHit _hit, Voxel _voxel, bool _adjacent = false)
    {
        Chunk chunk = _hit.collider.GetComponent<Chunk>();//try get chunk

        if (chunk == null)
            return false;//failed to get chunk

        intVector3 position = GetVoxelCentrePosition(_hit, _adjacent);
        chunk.voxel_world.SetVoxel(position.x, position.y, position.z, _voxel);

        return true;
    }


    public static Voxel GetVoxel(RaycastHit _hit, bool _adjacent = false)
    {
        Chunk chunk = _hit.collider.GetComponent<Chunk>();

        if (chunk == null)
            return null;

        intVector3 position = GetVoxelCentrePosition(_hit, _adjacent);
        Voxel voxel = chunk.voxel_world.GetVoxel(position.x, position.y, position.z);

        return voxel;
    }

}

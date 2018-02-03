using UnityEngine;
using System.Collections;
using System;


[Serializable]
public struct intVector3
{
    public int x, y, z;
    public intVector3(int _x, int _y, int _z)
    {
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }


    //for quicker comparison
    public override bool Equals(object _object)
    {
        if (!(_object is intVector3))//if not a world position
            return false;//ignore

        intVector3 position = (intVector3)_object;//cast to world position
        return position.x == x && position.y == y && position.z == z;
    }
}
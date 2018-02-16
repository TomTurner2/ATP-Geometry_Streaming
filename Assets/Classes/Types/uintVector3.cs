
using UnityEngine.VR.WSA.Persistence;

[System.Serializable]
struct uintVector3
{
    public uint x;
    public uint y;
    public uint z;

    public uintVector3(uint _x, uint _y, uint _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static readonly uintVector3 Zero = new uintVector3(0,0,0);
    public static readonly uintVector3 Up = new uintVector3(0, 1, 0);
    public static readonly uintVector3 Right = new uintVector3(1, 0, 0);
};
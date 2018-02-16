using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo
{
    private static string world_name = "World";

    public static string WorldName
    {
        get
        {
            return world_name;
        }
        set
        {
            world_name = value;
        }
    }

}

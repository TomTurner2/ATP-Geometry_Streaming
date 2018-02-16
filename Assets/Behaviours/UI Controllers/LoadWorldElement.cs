using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadWorldElement : MonoBehaviour
{
    private string world_name = "";


    private void Start()
    {
        GetComponentInChildren<Text>().text = world_name;
    }


    public string WorldName
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


    public void LoadWorld()
    {
        GameInfo.WorldName = world_name;
        SceneManager.LoadScene(1);
    }


    public void DeleteWorld()
    {
        VoxelWorldSaver.DeleteSave(world_name);
        Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    public static MeshManager instance = null;

    [SerializeField] List<GameObject> meshes = new List<GameObject>();


    public GameObject GetMeshPrefabByName(string _name)
    {
        return meshes.Find(m => m.name == _name);
    }


    private void Start()
    {
        if (instance == null)
        {
            Initialise();
        }
        else
        {
            Destroy(this);
        }
    }


    private void Initialise()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
}

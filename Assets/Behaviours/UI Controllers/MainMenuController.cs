using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] InputField world_name_input = null;
    [SerializeField] GameObject scroll_view_content = null;
    [SerializeField] GameObject load_world_button = null;


    private void Start()
    {
        LoadExistingSaves();
    }


    private void LoadExistingSaves()
    {
        string[] world_saves = VoxelWorldSaver.GetAllWorldSaves();

        foreach (string save in world_saves)
        {
            LoadWorldElement load_button = Instantiate(load_world_button, scroll_view_content.transform).GetComponent<LoadWorldElement>();
            load_button.WorldName = save;
        }
    }


    public void SetWorldName(string _name)
    {
        GameInfo.WorldName = _name;
    }


    public void CreateWorld()
    {
        if (world_name_input.text == "")
            return;

        SceneManager.LoadScene(1);
    }


    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}

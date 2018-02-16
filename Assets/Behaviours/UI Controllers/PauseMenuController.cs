using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pause_menu = null;


    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }


    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        pause_menu.SetActive(!pause_menu.gameObject.activeInHierarchy);

        LockMouse(!pause_menu.gameObject.activeInHierarchy);//bit hacky
    }


    public void LockMouse(bool _locked)
    {
        if (_locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}

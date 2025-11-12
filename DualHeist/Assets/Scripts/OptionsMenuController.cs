using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenuController : MonoBehaviour
{
    public string PreviousScene;

    public PauseMenuController PauseScript;

    public MainMenuController MainScript;


    public void ReturnToPreviousScene()
    {
        if (PreviousScene == null)
        {
            PreviousScene = "MainMenu";
        }
        if (PreviousScene == "MainMenu")
        {
            MainScript.MainMenuUI.SetActive(true);
            MainScript.OptionsMenu.SetActive(false);
        }
        if (PreviousScene == "Level 1")
        {
            PauseScript.pauseMenuUI.SetActive(true);
            PauseScript.OptionsMenu.SetActive(false);
        }
        //else 
        //{
            //PauseScript.pauseMenuUI.SetActive(true);
            //PauseScript.OptionsMenu.SetActive(false);
        //}
    }
}

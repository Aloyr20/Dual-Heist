using System.Collections;
using System.Collections.Generic;
#if Unity_Editor
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    //public static bool GameIsPaused = false;

    [SerializeField] public GameObject pauseMenuUI;
    public GameObject InGameUI;

    [SerializeField] public GameObject OptionsMenu;

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0f)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        InGameUI.SetActive(true);
        Time.timeScale = 1f;
        //GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        InGameUI.SetActive(false);
        Time.timeScale = 0f;
        //GameIsPaused = true;
    }

    public void Options()
    {
        //Scene CurrentScene = SceneManager.GetActiveScene();
        //string SceneName = CurrentScene.name;
        OptionsMenuController.PreviousScene = "SampleScene";
        OptionsMenu.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }
}

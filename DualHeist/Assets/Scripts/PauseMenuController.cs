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
    public GameObject WinUI;
    public GameObject LoseUI;
    public GameObject EndingUI;
    [SerializeField] Animator SceneTransitionAnim;


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

        if (Input.GetKey(KeyCode.K))
        {
            WinGame();
        }

        if (Input.GetKey(KeyCode.L))
        {
            LoseGame();
        }

        if (Input.GetKey(KeyCode.E))
        {
            EndingUI.SetActive(true);
            StartCoroutine(EndingUIToMainMenuTimer());
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

    public void Reset()
    {
        Scene CurrentScene = SceneManager.GetActiveScene();
        string SceneName = CurrentScene.name;
        SceneManager.LoadScene(SceneName);
    }

    public void Options()
    {
        //Scene CurrentScene = SceneManager.GetActiveScene();
        //string SceneName = CurrentScene.name;
        OptionsMenu.GetComponent<OptionsMenuController>().PreviousScene = "SampleScene";
        OptionsMenu.SetActive(true);
        //pauseMenuUI.SetActive(false);
    }

    public void WinGame()
    {
        InGameUI.SetActive(false);
        WinUI.SetActive(true);
    }

    public void LoseGame()
    {
        InGameUI.SetActive(false);
        LoseUI.SetActive(true);
    }


    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    IEnumerator EndingUIToMainMenuTimer()
    {
        yield return new WaitForSeconds(3f);
        EndingUI.SetActive(false);
        SceneTransitionAnim.gameObject.SetActive(true);
        SceneTransitionAnim.Play("SceneTransitionLoadAnimation", 0, 0f);
        yield return new WaitForSeconds(5f);
        MainMenu();
        SceneTransitionAnim.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject OptionsMenu;
    [SerializeField] private Animator SceneTransitionAnim;
    [SerializeField] private string firstLevelName = "Level 1";

    public void Start()
    {
        Time.timeScale = 1f; // Changed from 0f to 1f to ensure animations play properly
    }

    public void PlayGame()
    {
        StartCoroutine(SceneTransition());
    }

    private IEnumerator SceneTransition()
    {
        // Play the transition animation
        if (SceneTransitionAnim != null)
        {
            SceneTransitionAnim.gameObject.SetActive(true);
            SceneTransitionAnim.Play("SceneTransitionLoadAnimation", 0, 0f);

            // Get the length of the animation
            float animationLength = SceneTransitionAnim.GetCurrentAnimatorStateInfo(0).length;

            // Wait for the animation to complete
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            // Fallback: wait a short moment if no animator is set
            yield return new WaitForSeconds(1f);
        }

        // Load the scene
        SceneManager.LoadScene(firstLevelName);
        Time.timeScale = 1f;
    }

    public void Options()
    {
        OptionsMenu.GetComponent<OptionsMenuController>().PreviousScene = "MainMenu";
        OptionsMenu.SetActive(true);
        MainMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
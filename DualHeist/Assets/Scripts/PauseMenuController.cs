using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] public GameObject pauseMenuUI;
    public GameObject InGameUI;
    [SerializeField] public GameObject OptionsMenu;
    public GameObject WinUI;
    public GameObject LoseUI;
    public GameObject EndingUI;
    [SerializeField] private Animator SceneTransitionAnim;
    [SerializeField] private string finalSceneName = "FinalScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // VR Input Properties
    public InputActionProperty pauseButton;
    public InputActionProperty uiSelectButton;
    public LineRenderer pointerLine;
    public Transform controllerTransform; // Assign your controller transform

    // UI Raycast
    private GraphicRaycaster raycaster;
    private UnityEngine.EventSystems.EventSystem eventSystem;
    private Camera vrCamera;

    private bool isFinalScene = false;
    private bool endingSequenceStarted = false;

    void Start()
    {
        // Check if this is the final scene
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == finalSceneName)
        {
            isFinalScene = true;
            StartCoroutine(FinalSceneSequence());
        }

        // Get UI components
        raycaster = FindFirstObjectByType<GraphicRaycaster>();
        eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        vrCamera = Camera.main;

        // Setup pointer line
        if (pointerLine != null)
        {
            pointerLine.positionCount = 2;
            pointerLine.useWorldSpace = true;
        }

        // Enable input actions
        pauseButton.action.Enable();
        uiSelectButton.action.Enable();
    }

    void OnEnable()
    {
        pauseButton.action.performed += OnPauseButtonPressed;
        uiSelectButton.action.performed += OnUISelectPressed;
    }

    void OnDisable()
    {
        pauseButton.action.performed -= OnPauseButtonPressed;
        uiSelectButton.action.performed -= OnUISelectPressed;
    }

    public void Update()
    {
        // Update pointer line and UI interaction
        UpdateUIRaycast();

        // Keep Escape key for testing in editor
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

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    private void OnUISelectPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Check if we're pointing at a UI button and click it
            var button = GetUIElementAtPointer();
            if (button != null)
            {
                button.onClick.Invoke();
                Debug.Log("VR UI Button clicked: " + button.name);
            }
        }
    }

    private void UpdateUIRaycast()
    {
        if (pointerLine == null || controllerTransform == null) return;

        // Cast ray from controller
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit;

        // Update line renderer
        pointerLine.SetPosition(0, controllerTransform.position);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            pointerLine.SetPosition(1, hit.point);

            // Visual feedback when pointing at UI
            var button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                // Change line color or add hover effect
                pointerLine.startColor = Color.green;
                pointerLine.endColor = Color.green;
            }
            else
            {
                pointerLine.startColor = Color.white;
                pointerLine.endColor = Color.white;
            }
        }
        else
        {
            // Ray didn't hit anything, extend line
            pointerLine.SetPosition(1, controllerTransform.position + controllerTransform.forward * 100f);
            pointerLine.startColor = Color.white;
            pointerLine.endColor = Color.white;
        }
    }

    private Button GetUIElementAtPointer()
    {
        if (controllerTransform == null) return null;

        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            // Check if we hit a button
            var button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                return button;
            }
        }

        return null;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        InGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        InGameUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Reset()
    {
        Scene CurrentScene = SceneManager.GetActiveScene();
        string SceneName = CurrentScene.name;
        SceneManager.LoadScene(SceneName);
    }

    public void Options()
    {
        OptionsMenu.GetComponent<OptionsMenuController>().PreviousScene = "SampleScene";
        OptionsMenu.SetActive(true);
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

    private IEnumerator FinalSceneSequence()
    {
        if (endingSequenceStarted) yield break;

        endingSequenceStarted = true;

        // Wait 10 seconds in the final scene
        yield return new WaitForSeconds(10f);

        // Show Ending UI
        if (EndingUI != null)
        {
            InGameUI.SetActive(false);
            EndingUI.SetActive(true);
        }

        // Wait 5 more seconds
        yield return new WaitForSeconds(5f);

        // Disable Ending UI and enable Win UI
        if (EndingUI != null)
            EndingUI.SetActive(false);

        if (WinUI != null)
            WinUI.SetActive(true);
    }

    public void MainMenu()
    {
        Debug.Log("MainMenu button clicked!");
        StartCoroutine(TransitionToMainMenu());
    }

    public void TestButton()
    {
        Debug.Log("Test button is working!");
    }

    private IEnumerator TransitionToMainMenu()
    {
        Debug.Log("Starting transition to main menu");

        // Play transition animation if available
        if (SceneTransitionAnim != null)
        {
            SceneTransitionAnim.gameObject.SetActive(true);
            SceneTransitionAnim.Play("SceneTransitionLoadAnimation", 0, 0f);

            // Get animation length
            float animationLength = SceneTransitionAnim.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log("Animation length: " + animationLength);
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            Debug.LogWarning("SceneTransitionAnim is not assigned!");
            // Fallback wait time
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Loading main menu: " + mainMenuSceneName);
        // Load main menu
        SceneManager.LoadScene(mainMenuSceneName);
        Time.timeScale = 1f;
    }
}
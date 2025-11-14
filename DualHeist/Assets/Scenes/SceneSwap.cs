using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwap : MonoBehaviour
{
    public string nextSceneName;
    [SerializeField] Animator SceneTransitionAnim;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure it's the player triggering
        {
            StartCoroutine(SceneTransition());
        }
    }

    private IEnumerator SceneTransition()
    {
        // Enable and play the transition animation
        SceneTransitionAnim.gameObject.SetActive(true);
        SceneTransitionAnim.Play("SceneTransitionLoadAnimation", 0, 0f);

        // Get the length of the animation
        float animationLength = SceneTransitionAnim.GetCurrentAnimatorStateInfo(0).length;

        // Wait for the animation to complete
        yield return new WaitForSeconds(animationLength);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
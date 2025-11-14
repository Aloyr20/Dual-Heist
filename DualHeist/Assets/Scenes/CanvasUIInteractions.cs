using UnityEngine;
using UnityEngine.UI;

public class CanvasUIInteractions : MonoBehaviour
{
    void Start()
    {
        // Ensure all buttons have Image components for sprite swapping
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            // Add Image if missing
            if (button.GetComponent<Image>() == null)
            {
                button.gameObject.AddComponent<Image>();
            }

            // Ensure sprite state is properly configured
            if (button.spriteState.highlightedSprite == null)
            {
                Debug.LogWarning($"Button '{button.name}' is missing highlighted sprite for VR interaction", button);
            }

            if (button.spriteState.pressedSprite == null)
            {
                Debug.LogWarning($"Button '{button.name}' is missing pressed sprite for VR interaction", button);
            }
        }
    }
}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRUIRaycaster : MonoBehaviour
{
    [Header("Ray Settings")]
    public Transform rayOrigin;           // Usually your controller tip
    public float maxDistance = 10f;
    public LineRenderer line;             // Optional laser

    [Header("Input (New Input System)")]
    public InputActionProperty clickAction; // Bind to your controller trigger

    [Header("UI Interaction")]
    public LayerMask uiLayerMask = 1 << 5; // Default UI layer

    private Button currentButton;
    private Image currentButtonImage;
    private Sprite originalSprite;
    private bool hadOriginal;

    void OnEnable()
    {
        if (clickAction.action != null)
            clickAction.action.Enable();
    }

    void OnDisable()
    {
        if (clickAction.action != null)
            clickAction.action.Disable();
        ClearHover(); // revert any highlight on disable
    }

    void Update()
    {
        if (rayOrigin == null) return;

        // 1) Raycast from controller for UI elements
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        Button hoveredButton = null;
        Image hoveredImage = null;

        if (Physics.Raycast(ray, out hit, maxDistance, uiLayerMask))
        {
            // Find Button component in the hit object or its children
            hoveredButton = hit.collider.GetComponent<Button>();
            if (hoveredButton == null)
                hoveredButton = hit.collider.GetComponentInParent<Button>();

            // Find Image component
            if (hoveredButton != null)
            {
                hoveredImage = hoveredButton.GetComponent<Image>();
                if (hoveredImage == null)
                    hoveredImage = hoveredButton.GetComponentInChildren<Image>();
            }

            // Update laser to hit point
            UpdateLaserPointer(ray.origin, hit.point);
        }
        else
        {
            // No UI hit, extend laser
            UpdateLaserPointer(ray.origin, ray.origin + ray.direction * maxDistance); // FIXED: ray.direction instead of ray.foxward
        }

        // 2) Handle hover state changes
        if (hoveredButton != currentButton)
        {
            ClearHover();
            SetHover(hoveredButton, hoveredImage);
        }

        // 3) Handle click interaction
        if (currentButton != null && clickAction.action != null && clickAction.action.WasPressedThisFrame())
        {
            ExecuteButtonClick();
        }
    }

    private void SetHover(Button btn, Image btnImage)
    {
        if (btn == null || btnImage == null) return;

        currentButton = btn;
        currentButtonImage = btnImage;

        // Store original sprite
        originalSprite = currentButtonImage.sprite;
        hadOriginal = true;

        // Apply highlighted sprite (from Button's sprite swap)
        if (btn.spriteState.highlightedSprite != null)
        {
            currentButtonImage.sprite = btn.spriteState.highlightedSprite;
        }

        // Optional: Change laser color when hovering over button
        if (line != null)
        {
            line.startColor = Color.green;
            line.endColor = Color.green;
        }
    }

    private void ClearHover()
    {
        if (currentButton == null || currentButtonImage == null) return;

        // Revert to original sprite
        if (hadOriginal)
        {
            currentButtonImage.sprite = originalSprite;
        }

        // Reset laser color
        if (line != null)
        {
            line.startColor = Color.white;
            line.endColor = Color.white;
        }

        currentButton = null;
        currentButtonImage = null;
        hadOriginal = false;
    }

    private void ExecuteButtonClick()
    {
        if (currentButton == null) return;

        // Apply pressed sprite briefly
        if (currentButton.spriteState.pressedSprite != null && currentButtonImage != null)
        {
            Sprite currentSprite = currentButtonImage.sprite;
            currentButtonImage.sprite = currentButton.spriteState.pressedSprite;

            // Revert after a short moment
            StartCoroutine(RevertSpriteAfterClick(currentSprite));
        }

        // Also invoke the onClick events directly
        currentButton.onClick?.Invoke();

        Debug.Log("Button clicked: " + currentButton.name);
    }

    private System.Collections.IEnumerator RevertSpriteAfterClick(Sprite originalSprite)
    {
        yield return new WaitForSeconds(0.1f);
        if (currentButtonImage != null && currentButton != null)
        {
            // Revert to highlighted sprite (if still hovering) or original sprite
            if (currentButton.spriteState.highlightedSprite != null)
            {
                currentButtonImage.sprite = currentButton.spriteState.highlightedSprite;
            }
            else
            {
                currentButtonImage.sprite = originalSprite;
            }
        }
    }

    private void UpdateLaserPointer(Vector3 start, Vector3 end)
    {
        if (!line) return;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
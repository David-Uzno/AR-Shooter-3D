using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public class CollectableUI : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    private Canvas targetCanvas;
    private GraphicRaycaster graphicRaycaster;
    private readonly List<RaycastResult> raycastResults = new();

    private void Reset()
    {
        RefreshReferences();
    }

    private void Awake()
    {
        RefreshReferences();

        if (targetImage == null)
        {
            Debug.LogWarning("CollectableUI requiere una referencia a un Image.", this);
            return;
        }
    }

    private void OnEnable()
    {
        RefreshReferences();
    }

    private void Start()
    {
        if (targetImage == null)
        {
            return;
        }

        if (targetCanvas == null)
        {
            Debug.LogWarning("CollectableUI requiere que el Image pertenezca a un Canvas.", this);
        }

        if (graphicRaycaster == null)
        {
            Debug.LogWarning("CollectableUI requiere un GraphicRaycaster en el Canvas del Image.", this);
        }

        if (!targetImage.raycastTarget)
        {
            Debug.LogWarning("CollectableUI: Image tenía Raycast Target desactivado; se activará por código.", this);
            targetImage.raycastTarget = true;
        }

        if (EventSystem.current == null)
        {
            Debug.LogWarning("CollectableUI requiere un EventSystem activo en la escena.", this);
        }
    }

    private void Update()
    {
        if (targetImage == null)
        {
            return;
        }

        bool touchPressedThisFrame = false;

        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (!touch.press.wasPressedThisFrame)
                {
                    continue;
                }

                touchPressedThisFrame = true;
                TryHandlePress(touch.position.ReadValue());
            }
        }

        if (!touchPressedThisFrame && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryHandlePress(Mouse.current.position.ReadValue());
        }
    }

    private void TryHandlePress(Vector2 screenPosition)
    {
        if (!CanProcessPress(screenPosition))
        {
            return;
        }

        if (IsTargetPressed(screenPosition))
        {
            Debug.Log("El elemento fue presionado.", this);
        }
    }

    private bool IsTargetPressed(Vector2 screenPosition)
    {
        if (EventSystem.current != null)
        {
            PointerEventData pointerEventData = new(EventSystem.current)
            {
                position = screenPosition
            };

            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject == targetImage.gameObject || result.gameObject.transform.IsChildOf(targetImage.transform))
                {
                    return true;
                }
            }

            return false;
        }

        Camera eventCamera = null;

        if (targetCanvas != null && targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = targetCanvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(targetImage.rectTransform, screenPosition, eventCamera);
    }

    private bool CanProcessPress(Vector2 screenPosition)
    {
        if (targetImage == null || !targetImage.isActiveAndEnabled)
        {
            return false;
        }

        Camera eventCamera = null;

        if (targetCanvas != null && targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = targetCanvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(targetImage.rectTransform, screenPosition, eventCamera);
    }

    private void RefreshReferences()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        targetCanvas = targetImage != null ? targetImage.canvas : null;
        graphicRaycaster = targetCanvas != null ? targetCanvas.GetComponent<GraphicRaycaster>() : null;
    }
}

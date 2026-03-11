using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public class CollectableUI : MonoBehaviour
{
    [SerializeField] private CollectableItem _item;   
    [SerializeField] private bool _consumeOnCollect = true;

    [Header("Dependencies")]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Player _player;

    private Canvas _targetCanvas;
    private GraphicRaycaster _graphicRaycaster;
    private readonly List<RaycastResult> _raycastResults = new();

    private void Reset()
    {
        RefreshReferences();
    }

    private void Awake()
    {
        RefreshReferences();

        if (_targetImage == null)
        {
            Debug.LogWarning("CollectableUI requiere una referencia a un Image.", this);
            return;
        }
        
        if (_player == null)
        {
            GameObject playerGameObject = GameObject.FindWithTag("Player");
            if (playerGameObject != null)
            {
                _player = playerGameObject.GetComponent<Player>();
            }
            else
            {
                Debug.LogWarning("CollectableUI no tiene referencia a Player y no se encontró GameObject con tag 'Player'.", this);
            }
        }
    }

    private void OnEnable()
    {
        RefreshReferences();
    }

    private void Start()
    {
        if (_targetImage == null)
        {
            return;
        }

        if (_targetCanvas == null)
        {
            Debug.LogWarning("CollectableUI requiere que el Image pertenezca a un Canvas.", this);
        }

        if (_graphicRaycaster == null)
        {
            Debug.LogWarning("CollectableUI requiere un GraphicRaycaster en el Canvas del Image.", this);
        }

        if (!_targetImage.raycastTarget)
        {
            Debug.LogWarning("CollectableUI: Image tenía Raycast Target desactivado; se activará por código.", this);
            _targetImage.raycastTarget = true;
        }

        if (EventSystem.current == null)
        {
            Debug.LogWarning("CollectableUI requiere un EventSystem activo en la escena.", this);
        }

        if (_item == null)
        {
            Debug.LogWarning("CollectableUI requiere un CollectableItem asignado para aplicar un efecto.", this);
        }
    }

    private void Update()
    {
        if (_targetImage == null)
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
            TryCollect();
        }
    }

    private void TryCollect()
    {
        if (_item == null)
        {
            Debug.LogWarning("CollectableUI no puede activar el ítem porque no tiene un CollectableItem asignado.", this);
            return;
        }

        if (_player == null)
        {
            _player = FindFirstObjectByType<Player>();
        }

        bool wasCollected = _item.TryCollect(new CollectableContext(this, _player));

        if (!wasCollected)
        {
            return;
        }

        Debug.Log($"{_item.DisplayName} fue recogido.", this);

        if (FlickerEffect.Instance != null)
        {
            FlickerEffect.Instance.PlayAndShow();
        }

        if (_consumeOnCollect)
        {
            gameObject.SetActive(false);
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

            _raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerEventData, _raycastResults);

            foreach (RaycastResult result in _raycastResults)
            {
                if (result.gameObject == _targetImage.gameObject || result.gameObject.transform.IsChildOf(_targetImage.transform))
                {
                    return true;
                }
            }

            return false;
        }

        Camera eventCamera = null;

        if (_targetCanvas != null && _targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = _targetCanvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(_targetImage.rectTransform, screenPosition, eventCamera);
    }

    private bool CanProcessPress(Vector2 screenPosition)
    {
        if (_targetImage == null || !_targetImage.isActiveAndEnabled)
        {
            return false;
        }

        Camera eventCamera = null;

        if (_targetCanvas != null && _targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = _targetCanvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(_targetImage.rectTransform, screenPosition, eventCamera);
    }

    private void RefreshReferences()
    {
        if (_targetImage == null)
        {
            _targetImage = GetComponent<Image>();
        }

        _targetCanvas = _targetImage != null ? _targetImage.canvas : null;
        _graphicRaycaster = _targetCanvas != null ? _targetCanvas.GetComponent<GraphicRaycaster>() : null;
    }
}

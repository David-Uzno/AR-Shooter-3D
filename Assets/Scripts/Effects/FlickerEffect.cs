using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlickerEffect : MonoBehaviour
{
    public static FlickerEffect Instance { get; private set; }

    [SerializeField] private Image _imageFlicker;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _frequency = 8f;
    [SerializeField] private bool _playOnStart = false;
    
    private Color _originalColor;
    private bool _hasCachedOriginalColor;
    private bool _isPlaying = false;
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CacheOriginalColor(true);
    }

    private void Start()
    {
        CacheOriginalColor();
        if (_playOnStart)
        {
            if (_imageFlicker != null)
            {
                _imageFlicker.gameObject.SetActive(true);
                _imageFlicker.enabled = true;
            }

            StartFlicker();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void CacheOriginalColor(bool force = false)
    {
        if (_imageFlicker == null)
        {
            return;
        }

        if (!force && _hasCachedOriginalColor)
        {
            return;
        }

        _originalColor = _imageFlicker.color;
        _hasCachedOriginalColor = true;
    }

    private void StartFlicker()
    {
        if (_imageFlicker == null || !_imageFlicker.gameObject.activeInHierarchy || !_imageFlicker.enabled)
            return;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        _currentCoroutine = StartCoroutine(FlickerCoroutine());
    }

    public void PlayAndShow()
    {
        if (_imageFlicker == null)
        {
            Debug.LogWarning("FlickerEffect requiere una referencia a Image.", this);
            return;
        }

        CacheOriginalColor();
        _imageFlicker.gameObject.SetActive(true);
        _imageFlicker.enabled = true;
        SetAlpha(0f);

        StartFlicker();
    }

    private IEnumerator FlickerCoroutine()
    {
        _isPlaying = true;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float flickerIntensity = 0.5f * (1f + Mathf.Sin(2f * Mathf.PI * _frequency * elapsed));
            float alpha = Mathf.Lerp(0f, _originalColor.a, flickerIntensity);
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Calcular dinámicamente la duración del fade-out basada en la duración de cada parpadeo (periodo = 1/frequency)
        float fadeOutDuration = (_frequency > 0f) ? (1f / _frequency) : 0f;
        if (fadeOutDuration > 0f)
        {
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeOutDuration)
            {
                float envelope = 1f - (fadeElapsed / fadeOutDuration); // de 1 a 0
                float flickerIntensity = 0.5f * (1f + Mathf.Sin(2f * Mathf.PI * _frequency * (elapsed + fadeElapsed)));
                float alpha = Mathf.Lerp(0f, _originalColor.a * envelope, flickerIntensity);
                SetAlpha(alpha);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        SetAlpha(0f);
        if (_imageFlicker != null)
        {
            _imageFlicker.gameObject.SetActive(false);
        }

        _isPlaying = false;
        _currentCoroutine = null;
    }

    private void SetAlpha(float alphaValue)
    {
        if (_imageFlicker == null || !_imageFlicker.gameObject.activeInHierarchy || !_imageFlicker.enabled)
            return;

        Color flickerColor = _imageFlicker.color;
        flickerColor.r = _originalColor.r;
        flickerColor.g = _originalColor.g;
        flickerColor.b = _originalColor.b;
        flickerColor.a = alphaValue;
        _imageFlicker.color = flickerColor;
    }

    private void OnValidate()
    {
        if (_duration < 0f) _duration = 0f;
        if (_frequency < 0f) _frequency = 0f;

        if (!_isPlaying)
        {
            CacheOriginalColor(true);
        }
    }

    public float GetTotalDuration()
    {
        float fadeOut = (_frequency > 0f) ? (1f / _frequency) : 0f;
        return _duration + fadeOut;
    }
}

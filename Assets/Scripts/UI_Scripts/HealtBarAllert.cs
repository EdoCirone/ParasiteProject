using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealtBarAllert : MonoBehaviour
{
    [Header("Referece")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private GameObject _alertImage;
    [SerializeField] private GameObject _alertVignette;

    [Header("Settings")]
    [SerializeField] private float _valueToAlert = 0.3f;
    [SerializeField] private float _timeinterval = 0.5f;
    [SerializeField] private float _pulseScale = 1.2f;
    [SerializeField] private float _maxAlphaVignette = 0.5f;

    private bool _isAlertActive = false;
    private CanvasGroup _alertVignetteCanvasGroup;
    private Tween _pulseTweenImg;
    private Tween _pulseTweenVignette;

    private void Awake()
    {

        if (_alertImage != null)
        {
            _alertImage.SetActive(false);
            _alertImage.transform.localScale = Vector3.one;
        }

        if (_alertVignette != null)
        {
            _alertVignetteCanvasGroup = _alertVignette.GetComponent<CanvasGroup>();
            if (_alertVignetteCanvasGroup == null)
            {
                _alertVignetteCanvasGroup = _alertVignette.AddComponent<CanvasGroup>();
            }
            _alertVignette.SetActive(false);
            _alertVignetteCanvasGroup.alpha = 0f;
        }

        if (_healthBar == null || _alertImage == null || _alertVignette == null)
        {
            Debug.LogError("HealthBar or AllertImage/Vignette reference is missing in HealtBarAllert script.");
            return;
        }
    }

    private void Update()
    {
        if (_healthBar == null || _alertImage == null || _alertVignette == null || _alertVignetteCanvasGroup) return;

        bool shouldAllertBeActive = _healthBar.value <= _valueToAlert;

        if (shouldAllertBeActive && !_isAlertActive)
        {
            StartAlert();
        }
        else if (!shouldAllertBeActive && _isAlertActive)
        {
            StopAlert();
        }
    }

    private void StartAlert()
    {
        _isAlertActive = true;
        _alertImage.SetActive(true);
        _alertImage.transform.localScale = Vector3.one; // Reset scale before starting the pulse animation
        
        _alertVignette.SetActive(true);
        _alertVignette.transform.localScale = Vector3.one; // Ensure vignette is visible and at normal scale
        
        _pulseTweenImg?.Kill(); // Ensure any existing tween is killed before starting a new one
        _pulseTweenVignette?.Kill();

        _pulseTweenImg =    _alertImage.transform.DOScale(_pulseScale, _timeinterval).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _pulseTweenVignette = _alertVignetteCanvasGroup.DOFade(_maxAlphaVignette, _timeinterval).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void StopAlert()
    {

        _pulseTweenImg?.Kill();
        _pulseTweenImg = null;

        _pulseTweenVignette?.Kill();
        _pulseTweenVignette = null;

        if (_alertImage != null)
        {
            _alertImage.SetActive(false);
            _alertImage.transform.localScale = Vector3.one;
        }

        if( _alertVignette != null)
        {
            _alertVignette.SetActive(false);
            _alertVignetteCanvasGroup.alpha = 0f;
        }

        _isAlertActive = false;
    }

    private void OnDisable()
    {
        StopAlert();
    }

}


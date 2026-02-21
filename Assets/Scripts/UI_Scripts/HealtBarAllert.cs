using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealtBarAllert : MonoBehaviour
{
    [Header("Referece")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private GameObject _allertImage;

    [Header("Settings")]
    [SerializeField] private float _valueToAllert = 0.3f;
    [SerializeField] private float _timeinterval = 0.5f;
    [SerializeField] private float _pulseScale = 1.2f;

    private bool _isAllertActive = false;
    private Tween _pulseTween;

    private void Awake()
    {

        if (_allertImage != null)
        {
            _allertImage.SetActive(false);
            _allertImage.transform.localScale = Vector3.one;
        }

        if (_healthBar == null || _allertImage == null)
        {
            Debug.LogError("HealthBar or AllertImage reference is missing in HealtBarAllert script.");
            return;
        }
    }

    private void Update()
    {
        if (_healthBar == null || _allertImage == null) return;

        bool shouldAllertBeActive = _healthBar.value <= _valueToAllert;

        if (shouldAllertBeActive && !_isAllertActive)
        {
            StartAllert();
        }
        else if (!shouldAllertBeActive && _isAllertActive)
        {
            StopAllert();
        }
    }

    private void StartAllert()
    {
        _isAllertActive = true;
        _allertImage.SetActive(true);
        _allertImage.transform.localScale = Vector3.one; // Reset scale before starting the pulse animation
        _pulseTween?.Kill(); // Ensure any existing tween is killed before starting a new one

        _pulseTween = _allertImage.transform.DOScale(_pulseScale, _timeinterval).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void StopAllert()
    {
        _pulseTween?.Kill();
        _pulseTween = null;

        if (_allertImage != null)
        {
            _allertImage.SetActive(false);
            _allertImage.transform.localScale = Vector3.one;
        }
        _isAllertActive = false;
    }

    private void OnDisable()
    {
        StopAllert();
    }

}


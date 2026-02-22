using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private RectTransform _mainMenuPanel;

    [Header("Animation Settings")]
    [SerializeField] private float _mainFadeDuration = 0.5f;
    [SerializeField] private Ease _ease = Ease.OutBack;

    [Header("SubMenu Animation Settings")]
    [SerializeField] private float _subFadeDuration = 0.5f;
    [SerializeField] private Ease _subMenuEase = Ease.OutBack;

    private bool _isSettingsOpen = false;
    private bool _isCreditsOpen = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (_creditsPanel != null)_creditsPanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(false);

        if (_mainMenuPanel != null)
        {
            _mainMenuPanel.DOKill(); // Ferma eventuali animazioni in corso
            _mainMenuPanel.localScale = Vector3.zero;
            _mainMenuPanel.DOScale(Vector3.one, _mainFadeDuration).SetEase(_ease);
        }
    }

    public void OnNewGameButton() => GameManager.Instance.StartNewGame();
    public void OnExitButton() => GameManager.Instance.ExitGame();

    public void OnCreditsButton()
    {
        CloseSubMenu(_settingsPanel, ref _isSettingsOpen); // Chiude le settings se aperte
        OpenSubMenu(_creditsPanel, ref _isCreditsOpen);
    }

    public void OnSettingsButton()
    {
        CloseSubMenu(_creditsPanel, ref _isCreditsOpen); // Chiude i credits se aperti
        OpenSubMenu(_settingsPanel, ref _isSettingsOpen);
    }

    public void OpenSubMenu(GameObject panel, ref bool isOpen)
    {
        if (panel == null) return;
        if (isOpen) return; // Evita di aprire più volte le settings

        panel.SetActive(true);
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOKill(); // Ferma eventuali animazioni in corso
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, _subFadeDuration).SetEase(_subMenuEase);
        }
        isOpen = true;
    }

    public void CloseSubMenu(GameObject panel, ref bool isOpen)
    {
        if (panel == null) return;
        if (!isOpen) return; // Evita di chiudere se non è aperto

        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOKill(); // Ferma eventuali animazioni in corso
            rect.DOScale(Vector3.zero, _subFadeDuration).SetEase(_subMenuEase).OnComplete(() =>
            {
                panel.SetActive(false);
            });
        }
        else 
            panel.SetActive(false);// Chiude il pannello specificato
        isOpen = false;

    }

    public void OnBackFromCredits() => CloseSubMenu(_creditsPanel, ref _isCreditsOpen);
    public void OnBackFromSettings() => CloseSubMenu(_settingsPanel, ref _isSettingsOpen);
}

using UnityEngine;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _scoreBoardPanel;
    [SerializeField] private RectTransform _mainMenuPanel;

    [Header("Animation Settings")]
    [SerializeField] private float _mainFadeDuration = 0.5f;
    [SerializeField] private Ease _mainEase = Ease.OutBack;

    [Header("SubMenu Animation Settings")]
    [SerializeField] private float _subFadeDuration = 0.5f;
    [SerializeField] private Ease _subMenuEase = Ease.OutBack;
    [SerializeField] private Ease _subMenuCloseEase = Ease.OutCubic;

    [Header("Audio")]
    [SerializeField] private AudioEventData onClickAudioEventData;

    private bool _isSettingsOpen;
    private bool _isCreditsOpen;
    private bool _isScoreBoardOpen;

    private void Start()
    {
        Time.timeScale = 1f;

        if (_creditsPanel != null) _creditsPanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
        if (_scoreBoardPanel != null) _scoreBoardPanel.SetActive(false);

        if (_mainMenuPanel == null) return;

        _mainMenuPanel.DOKill();
        _mainMenuPanel.localScale = Vector3.zero;
        _mainMenuPanel.DOScale(Vector3.one, _mainFadeDuration).SetEase(_mainEase);
    }

    public void OnNewGameButton()
    {
        PlayClickAudio();
        GameManager.Instance.StartNewGame();
    }

    public void OnExitButton()
    {
        PlayClickAudio();
        GameManager.Instance.ExitGame();
    }

    public void OnCreditsButton()
    {
        PlayClickAudio();
        CloseSubMenu(_scoreBoardPanel, ref _isScoreBoardOpen);
        CloseSubMenu(_settingsPanel, ref _isSettingsOpen);
        OpenSubMenu(_creditsPanel, ref _isCreditsOpen);
    }

    public void OnSettingsButton()
    {
        PlayClickAudio();
        CloseSubMenu(_scoreBoardPanel, ref _isScoreBoardOpen);
        CloseSubMenu(_creditsPanel, ref _isCreditsOpen);
        OpenSubMenu(_settingsPanel, ref _isSettingsOpen);
    }

    public void OnScoreBoardButton()
    {
        PlayClickAudio();
        CloseSubMenu(_creditsPanel, ref _isCreditsOpen);
        CloseSubMenu(_settingsPanel, ref _isSettingsOpen);
        OpenSubMenu(_scoreBoardPanel, ref _isScoreBoardOpen);
    }

    public void OnBackFromCredits()
    {
        PlayClickAudio();
        CloseSubMenu(_creditsPanel, ref _isCreditsOpen);
    }

    public void OnBackFromSettings()
    {
        PlayClickAudio();
        CloseSubMenu(_settingsPanel, ref _isSettingsOpen);
    }

    public void OnBackFromScoreBoard()
    {
        PlayClickAudio();
        CloseSubMenu(_scoreBoardPanel, ref _isScoreBoardOpen);
    }

    private void OpenSubMenu(GameObject panel, ref bool isOpen)
    {
        if (panel == null || isOpen) return;

        panel.SetActive(true);
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOKill();
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, _subFadeDuration).SetEase(_subMenuEase);
        }

        isOpen = true;
    }

    private void CloseSubMenu(GameObject panel, ref bool isOpen)
    {
        if (panel == null || !isOpen) return;

        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOKill();
            rect.DOScale(Vector3.zero, _subFadeDuration).SetEase(_subMenuCloseEase).OnComplete(() => panel.SetActive(false));
        }
        else
        {
            panel.SetActive(false);
        }

        isOpen = false;
    }

    private void PlayClickAudio()
    {
        if (!onClickAudioEventData) return;

        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager) return;

        audioManager.PlaySound(onClickAudioEventData, transform.position);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _scoreBoardPanel;

    [Header("UI Anim Elements")]
    [SerializeField] private RectTransform _title;
    [SerializeField] private RectTransform _buttonsLayout;
    [SerializeField] private List<RectTransform> _buttons;

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

    //Mi serve un dizionario per memorizzare le posizioni finali dei bottoni, cos� da poter animare correttamente perch� il layout mi cambia le posizioni in Start e anima tutto sullo stesso punto
    private readonly Dictionary<RectTransform, Vector2> _cachedPos = new();
    void Start()
    {
        Time.timeScale = 1f;

        if (_creditsPanel != null) _creditsPanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
        if (_scoreBoardPanel != null) _scoreBoardPanel.SetActive(false);

        if (_title != null)
        {
            _title.DOKill(); // Ferma eventuali animazioni in corso
            _title.localScale = Vector3.zero;
            _title.DOScale(Vector3.one, _mainFadeDuration).SetEase(_mainEase);
        }

        // Forza il layout a calcolare le posizioni dei bottoni prima di animarli, cos� da avere le posizioni corrette per l'animazione
        Canvas.ForceUpdateCanvases();
        if (_buttonsLayout != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonsLayout);

        //  Cache posizioni finali 
        _cachedPos.Clear();
        foreach (var rt in _buttons)
        {
            if (rt != null)
                _cachedPos[rt] = rt.anchoredPosition;
        }

        AnimButtons();

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

    public void AnimButtons()
    {
        if (_buttons == null || _buttons.Count == 0) return;

        for (int i = 0; i < _buttons.Count; i++)
        {
            var rect = _buttons[i];
            if (rect == null) continue;

            //  posizione calcolata dal layout
            if (!_cachedPos.TryGetValue(rect, out var endPos))
                endPos = rect.anchoredPosition;

            rect.DOKill();

            var cg = rect.GetComponent<CanvasGroup>();
            if (!cg) cg = rect.gameObject.AddComponent<CanvasGroup>();

            cg.DOKill();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // Posizione di partenza (20 unit� sotto la posizione finale)
            Vector2 startPos = endPos + new Vector2(0f, -20f);
            rect.anchoredPosition = startPos;

            float delay = 0.08f * i;

            // Animazione di fade-in con un leggero delay per ogni bottone, e al completamento rende il bottone interattivo
            cg.DOFade(1f, _mainFadeDuration).SetEase(Ease.OutQuad).SetDelay(delay)
              .OnComplete(() => { cg.interactable = true; cg.blocksRaycasts = true; });

            // Animazione di movimento con un leggero overshoot (OutBack) per un effetto pi� dinamico
            rect.DOAnchorPos(endPos, _mainFadeDuration).SetEase(Ease.OutBack).SetDelay(delay);
        }
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

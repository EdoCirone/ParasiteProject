using UnityEngine;
using DG.Tweening;


public class InGameMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private GameObject _exitConfirmPanel;
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Settings")]
    [SerializeField] private float _subFadeDuration = 0.5f;
    [SerializeField] private Ease _subMenuEase = Ease.OutBack;
    [SerializeField] private Ease _subMenuCloseEase = Ease.InBack;

    private RectTransform _optionMenuRect;
    private RectTransform _exitConfirmRect;
    private RectTransform _gameOverRect;

    private bool _isSettingsOpen = false;
    private bool _isExitConfirmOpen = false;
    private bool _isGameOverOpen = false;

    private void Start()
    {
        if (_optionMenu == null || _exitConfirmPanel == null || _gameOverPanel == null)
        {
            Debug.LogError("InGameMenuManager: Missing references to option menu or exit confirm or GameOver panel.");
            return;
        }

        _optionMenuRect = _optionMenu.GetComponent<RectTransform>();
        _exitConfirmRect = _exitConfirmPanel.GetComponent<RectTransform>();
        _gameOverRect = _gameOverPanel.GetComponent<RectTransform>();

        if (_optionMenuRect == null || _exitConfirmRect == null || _gameOverRect == null)
        {
            Debug.LogError("InGameMenuManager: Missing RectTransform components on option menu or exit confirm panel.");
            return;
        }

        _optionMenuRect.localScale = Vector3.zero; // Inizialmente nascosto e scalato a zero per l'animazione
        _optionMenu.SetActive(false);

        _exitConfirmRect.localScale = Vector3.zero; // Inizialmente nascosto e scalato a zero per l'animazione
        _exitConfirmPanel.SetActive(false);

        _gameOverRect.localScale = Vector3.zero; // Inizialmente nascosto e scalato a zero per l'animazione
        _gameOverRect.gameObject.SetActive(false);

        HandlePause();
    }

    public void OnPlayerDeath()
    {
        CloseAllPanels();
        OpenSubMenu(_gameOverPanel, _gameOverRect, ref _isGameOverOpen);
    }

    public void OnSettingsButton()
    {
        CloseAllPanels();
        OpenSubMenu(_optionMenu, _optionMenuRect, ref _isSettingsOpen);
    }

    public void OnExitButton()
    {
        CloseAllPanels();
        OpenSubMenu(_exitConfirmPanel, _exitConfirmRect, ref _isExitConfirmOpen);
    }

    public void CloseAllPanels()
    {
        CloseSubMenu(_optionMenu, _optionMenuRect, ref _isSettingsOpen);
        CloseSubMenu(_exitConfirmPanel, _exitConfirmRect, ref _isExitConfirmOpen);
        CloseSubMenu(_gameOverPanel, _gameOverRect, ref _isGameOverOpen);
        HandlePause();
    }

    private void OpenSubMenu(GameObject panel, RectTransform rect, ref bool isOpen)
    {
        if (panel == null || rect == null || isOpen) return;

        panel.SetActive(true);

        rect.DOKill();
        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one, _subFadeDuration)
            .SetEase(_subMenuEase)
            .SetUpdate(true);

        isOpen = true;
        HandlePause();
    }

    private void CloseSubMenu(GameObject panel, RectTransform rect, ref bool isOpen)
    {
        if (panel == null || rect == null || !isOpen) return;

        rect.DOKill();
        rect.DOScale(Vector3.zero, _subFadeDuration)
            .SetEase(_subMenuCloseEase)
            .SetUpdate(true)
            .OnComplete(() => panel.SetActive(false));

        isOpen = false;
        HandlePause();
    }
    private void HandlePause()
    {
        Time.timeScale = (_isExitConfirmOpen || _isSettingsOpen || _isGameOverOpen) ? 0f : 1f;// Pausa il gioco se un menu è aperto, altrimenti lo riprende
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ReturnToMenu();

    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        GameManager.Instance.StartNewGame();
    }
}

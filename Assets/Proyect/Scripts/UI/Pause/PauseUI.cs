using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject pausePanel;

    [Header("Panel de volumen")]
    [SerializeField] private GameObject volumePanel;

    [Header("Botones")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Paneles bloqueados durante pausa")]
    [SerializeField] private CanvasGroup decisionPanel;
    [SerializeField] private CanvasGroup dialoguePanel;
    [SerializeField] private CanvasGroup deathPanel;

    private bool isPaused;

    private void Start()
    {
        pausePanel.SetActive(false);

        if (volumePanel != null)
        {
            volumePanel.SetActive(false);
        }

        resumeButton.onClick.AddListener(ResumeGame);
        tutorialButton.onClick.AddListener(OpenTutorial);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetPanelInteraction(false);

        pausePanel.SetActive(true);
    }

    private void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetPanelInteraction(true);

        pausePanel.SetActive(false);

        if (volumePanel != null)
        {
            volumePanel.SetActive(false);
        }
    }

    private void SetPanelInteraction(bool enabled)
    {
        if (decisionPanel != null)
        {
            decisionPanel.interactable = enabled;
            decisionPanel.blocksRaycasts = enabled;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.interactable = enabled;
            dialoguePanel.blocksRaycasts = enabled;
        }

        if (deathPanel != null)
        {
            deathPanel.interactable = enabled;
            deathPanel.blocksRaycasts = enabled;
        }
    }

    private void OpenTutorial()
    {
        AudioManager.Instance.PlayButtonClick();

        Debug.Log("Abrir tutorial");
    }

    private void OpenSettings()
    {
        AudioManager.Instance.PlayButtonClick();

        if (volumePanel != null)
        {
            volumePanel.SetActive(true);
        }
    }

    private void ReturnToMenu()
    {
        AudioManager.Instance.PlayButtonClick();

        Time.timeScale = 1f;

        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        SceneTransitionManager.Instance.LoadScene("Menu");
    }

    private void QuitGame()
    {
        AudioManager.Instance.PlayButtonClick();

        Application.Quit();
    }
}
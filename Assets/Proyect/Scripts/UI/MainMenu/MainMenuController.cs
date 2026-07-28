using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Paneles")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject volumePanel;

    [Header("Intro")]
    [SerializeField] private ComicSequence introSequence;

    [SerializeField] private string firstGameplayScene = "OfficeFloor";

    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(OpenVolumePanel);
        quitButton.onClick.AddListener(QuitGame);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (volumePanel != null)
        {
            volumePanel.SetActive(false);
        }
    }

    private void PlayGame()
    {
        AudioManager.Instance.PlayButtonClick();

        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        ComicState.CurrentSequence = introSequence;
        ComicState.IsIntro = true;
        ComicState.IntroNextScene = firstGameplayScene;

        SceneTransitionManager.Instance.LoadScene("DecisionComic");
    }

    private void OpenVolumePanel()
    {
        AudioManager.Instance.PlayButtonClick();

        if (volumePanel != null)
        {
            volumePanel.SetActive(true);
        }
    }

    private void QuitGame()
    {
        AudioManager.Instance.PlayButtonClick();

        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
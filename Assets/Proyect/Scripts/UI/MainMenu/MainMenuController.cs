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
    [SerializeField] private GameObject optionsPanel;

    [Header("Intro")]
    [SerializeField] private ComicSequence introSequence;

    [SerializeField] private string firstGameplayScene = "OfficeFloor";

    private bool optionsOpen;

    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        quitButton.onClick.AddListener(QuitGame);

        RefreshPanels();
    }

    private void PlayGame()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        ComicState.CurrentSequence = introSequence;
        ComicState.IsIntro = true;
        ComicState.IntroNextScene = firstGameplayScene;

        SceneTransitionManager.Instance.LoadScene("DecisionComic");
    }

    private void ToggleOptions()
    {
        optionsOpen = !optionsOpen;
        RefreshPanels();
    }

    private void RefreshPanels()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!optionsOpen);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(optionsOpen);
        }
    }

    private void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
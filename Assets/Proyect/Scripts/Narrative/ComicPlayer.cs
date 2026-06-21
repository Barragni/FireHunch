using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicPlayer : MonoBehaviour
{
    [Header("Modo Intro")]
    [SerializeField] private bool isIntroSequence;

    [Header("Secuencia Intro")]
    [SerializeField] private ComicSequence introSequence;

    [Header("UI Comic")]
    [SerializeField] private Image panelImage;
    [SerializeField] private TMP_Text panelText;

    [Header("Panel de muerte")]
    [SerializeField] private DeathPanelUI deathPanelUI;

    [Header("Escena después de la intro")]
    [SerializeField] private string introNextScene;

    private ComicSequence currentSequence;
    private int currentPanel;

    private void Start()
    {
        if (deathPanelUI != null)
        {
            deathPanelUI.Panel.SetActive(false);

            deathPanelUI.RetryButton.onClick.RemoveAllListeners();
            deathPanelUI.MenuButton.onClick.RemoveAllListeners();

            deathPanelUI.RetryButton.onClick.AddListener(
                RetryDecision
            );

            deathPanelUI.MenuButton.onClick.AddListener(
                ReturnToMenu
            );
        }

        if (isIntroSequence)
        {
            currentSequence = introSequence;
        }
        else
        {
            currentSequence =
                DecisionState.SelectedOption.comicSequence;
        }

        currentPanel = 0;

        ShowCurrentPanel();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextPanel();
        }
    }

    private void ShowCurrentPanel()
    {
        panelImage.sprite =
            currentSequence.panels[currentPanel].image;

        panelText.text =
            currentSequence.panels[currentPanel].text;
    }

    private void NextPanel()
    {
        currentPanel++;

        if (currentPanel >= currentSequence.panels.Count)
        {
            HandleEnd();
            return;
        }

        ShowCurrentPanel();
    }

    private void HandleEnd()
    {
        if (isIntroSequence)
        {
            SceneManager.LoadScene(
                introNextScene
            );

            return;
        }

        DecisionOption option =
            DecisionState.SelectedOption;

        if (
            option.outcomeType ==
            DecisionOutcomeType.Death
        )
        {
            deathPanelUI.Panel.SetActive(true);

            deathPanelUI.FeedbackText.text =
                option.deathFeedback;

            return;
        }

        SceneManager.LoadScene(
            option.nextScene
        );
    }

    private void RetryDecision()
    {
        NarrativeState.ReturningFromDeath = true;

        SceneManager.LoadScene(
            "OfficeFloor"
        );
    }

    private void ReturnToMenu()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        SceneManager.LoadScene(
            "Menu"
        );
    }
}
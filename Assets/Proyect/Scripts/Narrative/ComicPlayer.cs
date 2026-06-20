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

    [Header("UI")]
    [SerializeField] private Image panelImage;

    [SerializeField] private TMP_Text panelText;

    [Header("Escena después de la intro")]
    [SerializeField] private string introNextScene;

    private ComicSequence currentSequence;

    private int currentPanel;

    private void Start()
    {
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
            EndSequence();
            return;
        }

        ShowCurrentPanel();
    }

    private void EndSequence()
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
            SceneManager.LoadScene(
                "DeathScene"
            );

            return;
        }

        SceneManager.LoadScene(
            option.nextScene
        );
    }
}
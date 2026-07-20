using System.Collections;
using TMPro;
using UnityEngine;

public class ComicPlayer : MonoBehaviour
{
    [Header("Modo Intro")]
    [SerializeField] private bool isIntroSequence;

    [Header("Secuencia Intro")]
    [SerializeField] private ComicSequence introSequence;

    [Header("Transición de páginas")]
    [SerializeField] private ComicPageTransition pageTransition;

    [Header("UI")]
    [SerializeField] private TMP_Text panelText;

    [Header("Panel de muerte")]
    [SerializeField] private DeathPanelUI deathPanelUI;

    [Header("Panel de decisión")]
    [SerializeField] private DecisionPlayer decisionPlayer;

    [Header("Escena después de la intro")]
    [SerializeField] private string introNextScene;

    private ComicSequence currentSequence;
    private int currentPanel;
    private bool changingPage;

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

        if (decisionPlayer != null)
        {
            decisionPlayer.gameObject.SetActive(true);
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

        pageTransition.SetFirstPage(
            currentSequence.panels[currentPanel].image
        );

        panelText.text =
            currentSequence.panels[currentPanel].text;
    }

    private void Update()
    {
        // Mientras se está reproduciendo la transición
        // de página no se aceptan clics.
        if (changingPage)
        {
            return;
        }

        // Si el panel de decisión está abierto,
        // el cómic deja de procesar clics y éstos
        // quedan exclusivamente para los botones.
        if (
            decisionPlayer != null &&
            decisionPlayer.IsDecisionOpen
        )
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(
                NextPanel()
            );
        }
    }

    private IEnumerator NextPanel()
    {
        currentPanel++;

        if (currentPanel >= currentSequence.panels.Count)
        {
            HandleEnd();
            yield break;
        }

        changingPage = true;

        AudioManager.Instance.PlayPageFlip();

        yield return StartCoroutine(
            pageTransition.Play(
                currentSequence.panels[currentPanel].image
            )
        );

        panelText.text =
            currentSequence.panels[currentPanel].text;

        changingPage = false;
    }

    private void HandleEnd()
    {
        if (isIntroSequence)
        {
            SceneTransitionManager.Instance.LoadScene(
                introNextScene
            );

            return;
        }

        // Si el ComicSequence tiene una decisión asociada,
        // se muestra inmediatamente al terminar la última viñeta.
        if (
            currentSequence.decisionAfterComic != null &&
            decisionPlayer != null
        )
        {
            decisionPlayer.ShowDecision(
                currentSequence.decisionAfterComic
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

        NarrativeManager.Instance.SetCurrentNode(
            option.nextNode
        );

        string nextScene =
            NarrativeManager.Instance.GetSceneForNode(
                option.nextNode
            );

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError(
                $"No se encontró una escena para el nodo '{option.nextNode}'."
            );

            return;
        }

        SceneTransitionManager.Instance.LoadScene(
            nextScene
        );
    }

    private void RetryDecision()
    {
        NarrativeState.ReturningFromDeath = true;

        NarrativeState.SkipDialogue = true;

        SceneTransitionManager.Instance.LoadScene(
            NarrativeState.ReturnScene
        );
    }

    private void ReturnToMenu()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        SceneTransitionManager.Instance.LoadScene(
            "Menu"
        );
    }
}
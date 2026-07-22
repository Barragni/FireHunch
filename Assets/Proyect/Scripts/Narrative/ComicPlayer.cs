using System.Collections;
using UnityEngine;

public class ComicPlayer : MonoBehaviour
{
    [Header("Transición de páginas")]
    [SerializeField] private ComicPageTransition pageTransition;

    [Header("Panel de muerte")]
    [SerializeField] private DeathPanelUI deathPanelUI;

    [Header("Panel de decisión")]
    [SerializeField] private DecisionPlayer decisionPlayer;

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

            deathPanelUI.RetryButton.onClick.AddListener(RetryDecision);
            deathPanelUI.MenuButton.onClick.AddListener(ReturnToMenu);
        }

        if (decisionPlayer != null)
        {
            decisionPlayer.gameObject.SetActive(true);
        }

        currentSequence = ComicState.CurrentSequence;

        if (currentSequence == null)
        {
            Debug.LogError("ComicPlayer: ComicState.CurrentSequence es NULL.");
            return;
        }

        StartSequence(currentSequence);
    }

    private void Update()
    {
        if (changingPage)
            return;

        if (decisionPlayer != null && decisionPlayer.IsDecisionOpen)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(NextPanel());
        }
    }

    private void StartSequence(ComicSequence sequence)
    {
        currentSequence = sequence;
        currentPanel = 0;

        pageTransition.SetFirstPage(
            currentSequence.panels[currentPanel].image,
            currentSequence.panels[currentPanel].text
        );
    }

    private IEnumerator NextPanel()
    {
        currentPanel++;

        if (currentPanel >= currentSequence.panels.Count)
        {
            yield return StartCoroutine(HandleEnd());
            yield break;
        }

        changingPage = true;

        AudioManager.Instance.PlayPageFlip();

        yield return StartCoroutine(
            pageTransition.Play(
                currentSequence.panels[currentPanel].image,
                currentSequence.panels[currentPanel].text
            )
        );

        changingPage = false;
    }

    private IEnumerator HandleEnd()
    {
        // INTRO
        if (ComicState.IsIntro)
        {
            ComicState.IsIntro = false;
            ComicState.CurrentSequence = null;

            SceneTransitionManager.Instance.LoadScene(
                ComicState.IntroNextScene
            );

            yield break;
        }

        // DECISIÓN DESPUÉS DEL CÓMIC
        if (currentSequence.decisionAfterComic != null)
        {
            decisionPlayer.ShowDecision(currentSequence.decisionAfterComic);
            yield break;
        }

        // CÓMIC FINAL
        if (currentSequence.endingComic != null)
        {
            changingPage = true;

            ComicSequence nextComic = currentSequence.endingComic;

            yield return StartCoroutine(
                SceneTransitionManager.Instance.PlayFadeOnly(() =>
                {
                    ComicState.CurrentSequence = nextComic;
                    StartSequence(nextComic);
                })
            );

            changingPage = false;
            yield break;
        }

        // RESULTADOS
        if (currentSequence.endsWithResults)
        {
            SceneTransitionManager.Instance.LoadScene("Results");
            yield break;
        }

        // A partir de aquí ya debe existir una decisión.
        if (DecisionState.SelectedOption == null)
        {
            Debug.LogError("ComicPlayer: No existe DecisionState.SelectedOption.");
            yield break;
        }

        DecisionOption option = DecisionState.SelectedOption;

        if (option.outcomeType == DecisionOutcomeType.Death)
        {
            deathPanelUI.Panel.SetActive(true);
            deathPanelUI.FeedbackText.text = option.deathFeedback;
            yield break;
        }

        NarrativeManager.Instance.SetCurrentNode(option.nextNode);

        string nextScene =
            NarrativeManager.Instance.GetSceneForNode(option.nextNode);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError($"No se encontró una escena para el nodo '{option.nextNode}'.");
            yield break;
        }

        ComicState.CurrentSequence = null;

        SceneTransitionManager.Instance.LoadScene(nextScene);
    }

    private void RetryDecision()
    {
        NarrativeState.ReturningFromDeath = true;
        NarrativeState.SkipDialogue = true;

        ComicState.CurrentSequence = DecisionState.SelectedOption.comicSequence;
        ComicState.IsIntro = false;

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

        ComicState.CurrentSequence = null;
        ComicState.IsIntro = false;
        ComicState.IntroNextScene = string.Empty;

        SceneTransitionManager.Instance.LoadScene("Menu");
    }
}
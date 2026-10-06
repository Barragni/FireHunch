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

        currentSequence =
            ComicState.CurrentSequence;

        if (currentSequence == null)
        {
            Debug.LogError(
                "ComicPlayer: ComicState.CurrentSequence es NULL."
            );

            return;
        }

        StartSequence(
            currentSequence
        );
    }


    private void Update()
    {
        if (changingPage)
            return;

        if (
            SceneTransitionManager.Instance != null &&
            SceneTransitionManager.Instance.IsTransitioning
        )
        {
            return;
        }

        if (
            deathPanelUI != null &&
            deathPanelUI.Panel != null &&
            deathPanelUI.Panel.activeSelf
        )
        {
            return;
        }

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


    private void StartSequence(
        ComicSequence sequence
    )
    {
        currentSequence =
            sequence;

        currentPanel = 0;

        pageTransition.SetFirstPage(
            currentSequence.panels[
                currentPanel
            ].image,

            currentSequence.panels[
                currentPanel
            ].text
        );
    }


    private IEnumerator NextPanel()
    {
        currentPanel++;

        if (
            currentPanel >=
            currentSequence.panels.Count
        )
        {
            yield return StartCoroutine(
                HandleEnd()
            );

            yield break;
        }

        changingPage = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPageFlip();
        }

        yield return StartCoroutine(
            pageTransition.Play(
                currentSequence.panels[
                    currentPanel
                ].image,

                currentSequence.panels[
                    currentPanel
                ].text
            )
        );

        changingPage = false;
    }


    private IEnumerator HandleEnd()
    {

        if (ComicState.IsIntro)
        {
            ComicState.IsIntro = false;

            ComicState.CurrentSequence =
                null;

            SceneTransitionManager.Instance.LoadScene(
                ComicState.IntroNextScene
            );

            yield break;
        }


        if (
            currentSequence.decisionAfterComic != null
        )
        {
            decisionPlayer.ShowDecision(
                currentSequence.decisionAfterComic
            );

            yield break;
        }


        if (
            currentSequence.endingComic != null
        )
        {
            changingPage = true;

            ComicSequence nextComic =
                currentSequence.endingComic;

            yield return StartCoroutine(
                SceneTransitionManager.Instance.PlayFadeOnly(
                    () =>
                    {
                        ComicState.CurrentSequence =
                            nextComic;

                        StartSequence(
                            nextComic
                        );
                    }
                )
            );

            changingPage = false;

            yield break;
        }


        if (
            currentSequence.endsWithResults
        )
        {
            SceneTransitionManager.Instance.LoadScene(
                "Results"
            );

            yield break;
        }


        if (
            DecisionState.SelectedOption == null
        )
        {
            Debug.LogError(
                "ComicPlayer: No existe DecisionState.SelectedOption."
            );

            yield break;
        }

        DecisionOption option =
            DecisionState.SelectedOption;


        if (
            option.outcomeType ==
            DecisionOutcomeType.Death
        )
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeathMusic();
            }

            deathPanelUI.FeedbackText.text =
                option.deathFeedback;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            deathPanelUI.Show();

            yield break;
        }


        NarrativeManager.Instance.SetCurrentNode(
            option.nextNode
        );

        string nextScene =
            NarrativeManager.Instance.GetSceneForNode(
                option.nextNode
            );

        if (
            string.IsNullOrEmpty(nextScene)
        )
        {
            Debug.LogError(
                $"No se encontró una escena para el nodo '{option.nextNode}'."
            );

            yield break;
        }

        ComicState.CurrentSequence =
            null;

        SceneTransitionManager.Instance.LoadScene(
            nextScene
        );
    }


    private void RetryDecision()
    {
        StartCoroutine(
            RetryDecisionRoutine()
        );
    }

    private IEnumerator RetryDecisionRoutine()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();

            AudioManager.Instance.StopMusic();
        }


        if (deathPanelUI != null)
        {
            yield return StartCoroutine(
                deathPanelUI.HideAnimated()
            );
        }


        NarrativeState.ReturningFromDeath =
            true;

        NarrativeState.SkipDialogue =
            true;


        ComicState.CurrentSequence =
            null;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;


        DecisionState.SelectedOption =
            null;


        if (
            string.IsNullOrEmpty(
                NarrativeState.ReturnScene
            )
        )
        {
            Debug.LogError(
                "ComicPlayer: No existe ReturnScene para el reintento."
            );

            NarrativeState.ReturningFromDeath =
                false;

            yield break;
        }


        SceneTransitionManager.Instance.LoadScene(
            NarrativeState.ReturnScene
        );
    }


    private void ReturnToMenu()
    {
        StartCoroutine(
            ReturnToMenuRoutine()
        );
    }

    private IEnumerator ReturnToMenuRoutine()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();

            AudioManager.Instance.StopMusic();

            AudioManager.Instance.StopAmbience();
        }


        if (deathPanelUI != null)
        {
            yield return StartCoroutine(
                deathPanelUI.HideAnimated()
            );
        }


        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }


        ComicState.CurrentSequence =
            null;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;


        DecisionState.CurrentDecision =
            null;

        DecisionState.SelectedOption =
            null;


        NarrativeState.PendingDecision =
            null;

        NarrativeState.ReturningFromDeath =
            false;

        NarrativeState.SkipDialogue =
            false;

        NarrativeState.ReturnScene =
            string.Empty;

        NarrativeState.ReturnNodeID =
            string.Empty;


        SceneTransitionManager.Instance.LoadScene(
            "Menu"
        );
    }
}
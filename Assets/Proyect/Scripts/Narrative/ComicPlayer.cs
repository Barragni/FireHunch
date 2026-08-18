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
        // ========================================================
        // INTRO
        // ========================================================

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

        // ========================================================
        // DECISIÓN DESPUÉS DEL CÓMIC
        // ========================================================

        if (
            currentSequence.decisionAfterComic != null
        )
        {
            decisionPlayer.ShowDecision(
                currentSequence.decisionAfterComic
            );

            yield break;
        }

        // ========================================================
        // CÓMIC FINAL
        // ========================================================

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

        // ========================================================
        // RESULTADOS
        // ========================================================

        if (
            currentSequence.endsWithResults
        )
        {
            SceneTransitionManager.Instance.LoadScene(
                "Results"
            );

            yield break;
        }

        // ========================================================
        // A PARTIR DE AQUÍ DEBE EXISTIR UNA DECISIÓN
        // ========================================================

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

        // ========================================================
        // MUERTE
        // ========================================================

        if (
            option.outcomeType ==
            DecisionOutcomeType.Death
        )
        {
            deathPanelUI.Panel.SetActive(true);

            deathPanelUI.FeedbackText.text =
                option.deathFeedback;

            yield break;
        }

        // ========================================================
        // SIGUIENTE NODO
        // ========================================================

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

    // ============================================================
    // REINTENTAR
    // ============================================================

    private void RetryDecision()
    {
        // ========================================================
        // INDICAR QUE VOLVEMOS DE UNA MUERTE
        // ========================================================

        NarrativeState.ReturningFromDeath =
            true;

        NarrativeState.SkipDialogue =
            true;

        // ========================================================
        // CONSERVAR LA DECISIÓN QUE SE ESTABA RESOLVIENDO
        // ========================================================

        // PendingDecision ya fue guardada por DecisionPlayer
        // antes de salir hacia DecisionComic.
        //
        // NO debemos reemplazar ComicState.CurrentSequence
        // con el comic de la opción mortal.
        //
        // Al volver a la escena, DecisionResumeManager utilizará
        // NarrativeState.PendingDecision.
        // ========================================================

        ComicState.CurrentSequence =
            null;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;

        // ========================================================
        // LA OPCIÓN MORTAL YA NO DEBE QUEDAR COMO SELECCIONADA
        // ========================================================

        DecisionState.SelectedOption =
            null;

        // ========================================================
        // VOLVER A LA ESCENA DONDE ESTABA LA DECISIÓN
        // ========================================================

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

            return;
        }

        SceneTransitionManager.Instance.LoadScene(
            NarrativeState.ReturnScene
        );
    }

    // ============================================================
    // VOLVER AL MENÚ
    // ============================================================

    private void ReturnToMenu()
    {
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
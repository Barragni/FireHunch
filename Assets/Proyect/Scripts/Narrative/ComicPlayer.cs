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

    // ============================================================
    // START
    // ============================================================

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

    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (changingPage)
            return;

        // No permitir clics mientras se está realizando
        // una transición de escena o un fade.
        if (
            SceneTransitionManager.Instance != null &&
            SceneTransitionManager.Instance.IsTransitioning
        )
        {
            return;
        }

        // Si el panel de muerte está abierto,
        // ignorar cualquier clic en la pantalla.
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

    // ============================================================
    // INICIAR SECUENCIA
    // ============================================================

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

    // ============================================================
    // SIGUIENTE PANEL
    // ============================================================

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

    // ============================================================
    // FINAL DEL CÓMIC
    // ============================================================

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
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeathMusic();
            }

            deathPanelUI.FeedbackText.text =
                option.deathFeedback;

            deathPanelUI.Show();

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
        StartCoroutine(
            RetryDecisionRoutine()
        );
    }

    private IEnumerator RetryDecisionRoutine()
    {
        // ========================================================
        // SONIDO DEL BOTÓN
        // ========================================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();

            // Detener inmediatamente la música de muerte.
            AudioManager.Instance.StopMusic();
        }

        // ========================================================
        // ANIMACIÓN INVERSA
        // ========================================================

        if (deathPanelUI != null)
        {
            yield return StartCoroutine(
                deathPanelUI.HideAnimated()
            );
        }

        // ========================================================
        // INDICAR QUE VOLVEMOS DE UNA MUERTE
        // ========================================================

        NarrativeState.ReturningFromDeath =
            true;

        NarrativeState.SkipDialogue =
            true;

        // ========================================================
        // LIMPIAR ESTADO DEL CÓMIC
        // ========================================================

        ComicState.CurrentSequence =
            null;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;

        // ========================================================
        // LIMPIAR OPCIÓN MORTAL
        // ========================================================

        DecisionState.SelectedOption =
            null;

        // ========================================================
        // COMPROBAR ESCENA DE RETORNO
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

            yield break;
        }

        // ========================================================
        // VOLVER A LA ESCENA
        // ========================================================

        SceneTransitionManager.Instance.LoadScene(
            NarrativeState.ReturnScene
        );
    }

    // ============================================================
    // VOLVER AL MENÚ
    // ============================================================

    private void ReturnToMenu()
    {
        StartCoroutine(
            ReturnToMenuRoutine()
        );
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        // ========================================================
        // SONIDO DEL BOTÓN
        // ========================================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();

            AudioManager.Instance.StopMusic();

            AudioManager.Instance.StopAmbience();
        }

        // ========================================================
        // ANIMACIÓN INVERSA
        // ========================================================

        if (deathPanelUI != null)
        {
            yield return StartCoroutine(
                deathPanelUI.HideAnimated()
            );
        }

        // ========================================================
        // REINICIAR GAME STATE
        // ========================================================

        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        // ========================================================
        // LIMPIAR ESTADO DEL CÓMIC
        // ========================================================

        ComicState.CurrentSequence =
            null;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;

        // ========================================================
        // LIMPIAR DECISIÓN
        // ========================================================

        DecisionState.CurrentDecision =
            null;

        DecisionState.SelectedOption =
            null;

        // ========================================================
        // LIMPIAR ESTADO NARRATIVO
        // ========================================================

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

        // ========================================================
        // VOLVER AL MENÚ
        // ========================================================

        SceneTransitionManager.Instance.LoadScene(
            "Menu"
        );
    }
}
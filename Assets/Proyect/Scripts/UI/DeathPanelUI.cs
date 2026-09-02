using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Contenido")]
    [SerializeField] private TMP_Text feedbackText;

    [Header("Botones")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    // ============================================================
    // ANIMACIÓN DE APARICIÓN
    // ============================================================

    [Header("Animación de muerte")]

    [Tooltip(
        "Duración total de la animación de aparición."
    )]
    [SerializeField] private float animationDuration = 0.75f;

    [Tooltip(
        "Escala inicial respecto al tamaño original del panel."
    )]
    [SerializeField] private float startScale = 0.82f;

    [Tooltip(
        "Pequeño exceso de tamaño antes de volver al tamaño normal."
    )]
    [SerializeField] private float overshootScale = 1.03f;

    // ============================================================
    // ANIMACIÓN DE CIERRE
    // ============================================================

    [Header("Animación de cierre")]

    [Tooltip(
        "Duración de la animación al cerrar el panel."
    )]
    [SerializeField] private float hideAnimationDuration = 0.45f;

    [Tooltip(
        "Escala mínima a la que llega el panel al cerrarse."
    )]
    [SerializeField] private float hideEndScale = 0.82f;

    // ============================================================
    // VARIABLES
    // ============================================================

    private RectTransform panelRect;

    private CanvasGroup panelCanvasGroup;

    private Vector3 originalScale;

    private Coroutine animationRoutine;

    // ============================================================
    // PROPIEDADES
    // ============================================================

    public GameObject Panel =>
        panel;

    public TMP_Text FeedbackText =>
        feedbackText;

    public Button RetryButton =>
        retryButton;

    public Button MenuButton =>
        menuButton;

    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (panel == null)
        {
            Debug.LogError(
                "DeathPanelUI: Panel no está asignado."
            );

            return;
        }

        panelRect =
            panel.GetComponent<RectTransform>();

        if (panelRect == null)
        {
            Debug.LogError(
                "DeathPanelUI: El Panel debe tener un RectTransform."
            );

            return;
        }

        // ========================================================
        // GUARDAR ESCALA ORIGINAL
        // ========================================================
        //
        // Esta es la escala REAL que tienes configurada en Unity.
        // La animación siempre vuelve exactamente a ella.
        // ========================================================

        originalScale =
            panelRect.localScale;

        // ========================================================
        // CANVAS GROUP
        // ========================================================

        panelCanvasGroup =
            panel.GetComponent<CanvasGroup>();

        if (panelCanvasGroup == null)
        {
            panelCanvasGroup =
                panel.AddComponent<CanvasGroup>();
        }

        // ========================================================
        // ESTADO INICIAL
        // ========================================================

        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;
    }

    // ============================================================
    // MOSTRAR
    // ============================================================

    public void Show()
    {
        if (panel == null)
            return;

        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );
        }

        panel.SetActive(true);

        animationRoutine =
            StartCoroutine(
                ShowAnimation()
            );
    }

    // ============================================================
    // OCULTAR INMEDIATAMENTE
    // ============================================================

    public void Hide()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }

        if (panelRect != null)
        {
            panelRect.localScale =
                originalScale;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    // ============================================================
    // OCULTAR CON ANIMACIÓN
    // ============================================================

    public IEnumerator HideAnimated()
    {
        if (
            panel == null ||
            panelRect == null ||
            panelCanvasGroup == null
        )
        {
            yield break;
        }

        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }

        float duration =
            Mathf.Max(
                0.01f,
                hideAnimationDuration
            );

        float elapsed = 0f;

        Vector3 currentScale =
            panelRect.localScale;

        float currentAlpha =
            panelCanvasGroup.alpha;

        float targetScale =
            Mathf.Clamp01(
                hideEndScale
            );

        // ========================================================
        // ANIMACIÓN INVERSA
        // ========================================================
        //
        // El panel se desvanece mientras vuelve hacia el centro
        // y se hace pequeño.
        // ========================================================

        while (
            elapsed <
            duration
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );

            // ----------------------------------------------------
            // CURVA SUAVE
            // ----------------------------------------------------

            float smooth =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            // ----------------------------------------------------
            // FADE OUT
            // ----------------------------------------------------

            panelCanvasGroup.alpha =
                Mathf.Lerp(
                    currentAlpha,
                    0f,
                    smooth
                );

            // ----------------------------------------------------
            // REDUCCIÓN HACIA EL CENTRO
            // ----------------------------------------------------

            panelRect.localScale =
                Vector3.Lerp(
                    currentScale,
                    originalScale *
                    targetScale,
                    smooth
                );

            yield return null;
        }

        // ========================================================
        // ESTADO FINAL
        // ========================================================

        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale;

        panel.SetActive(false);

        animationRoutine = null;
    }

    // ============================================================
    // ANIMACIÓN DE APARICIÓN
    // ============================================================

    private IEnumerator ShowAnimation()
    {
        float duration =
            Mathf.Max(
                0.01f,
                animationDuration
            );

        float elapsed = 0f;

        // ========================================================
        // ESTADO INICIAL
        // ========================================================

        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;

        // ========================================================
        // PRIMERA PARTE
        // ========================================================

        float firstPhase =
            duration * 0.72f;

        while (
            elapsed <
            firstPhase
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    firstPhase
                );

            float smooth =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            panelCanvasGroup.alpha =
                smooth;

            panelRect.localScale =
                originalScale *
                Mathf.Lerp(
                    startScale,
                    overshootScale,
                    smooth
                );

            yield return null;
        }

        // ========================================================
        // SEGUNDA PARTE
        // ========================================================

        float secondPhase =
            duration * 0.28f;

        elapsed = 0f;

        while (
            elapsed <
            secondPhase
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    secondPhase
                );

            float smooth =
                1f -
                Mathf.Pow(
                    1f - t,
                    2f
                );

            panelRect.localScale =
                originalScale *
                Mathf.Lerp(
                    overshootScale,
                    1f,
                    smooth
                );

            yield return null;
        }

        // ========================================================
        // ESTADO FINAL
        // ========================================================

        panelCanvasGroup.alpha = 1f;

        panelRect.localScale =
            originalScale;

        animationRoutine = null;
    }
}
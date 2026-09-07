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
        "Duración de la animación de cierre."
    )]
    [SerializeField] private float hideAnimationDuration = 0.75f;

    // ============================================================
    // VARIABLES
    // ============================================================

    private RectTransform panelRect;

    private CanvasGroup panelCanvasGroup;

    private Vector3 originalScale;

    private Coroutine animationRoutine;

    // Indica que el panel ya está mostrando la muerte.
    // Impide que Show() vuelva a iniciar la animación.
    private bool deathShown;

    // Indica que el panel se encuentra en proceso de cierre.
    private bool isHiding;

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

        deathShown = false;
        isHiding = false;
    }

    // ============================================================
    // MOSTRAR
    // ============================================================

    public void Show()
    {
        if (
            panel == null ||
            panelRect == null ||
            panelCanvasGroup == null
        )
        {
            return;
        }

        // ========================================================
        // PROTECCIÓN CONTRA REPETICIÓN
        // ========================================================
        //
        // Si la muerte ya fue mostrada, NO volver a reproducir
        // la animación.
        //
        // Esto evita que un clic vuelva a hacer aparecer el panel.
        // ========================================================

        if (deathShown)
        {
            return;
        }

        // Si está cerrándose, tampoco permitir una nueva aparición.
        if (isHiding)
        {
            return;
        }

        deathShown = true;

        // ========================================================
        // DETENER CUALQUIER ANIMACIÓN ANTERIOR
        // ========================================================

        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }

        // ========================================================
        // MOSTRAR PANEL
        // ========================================================

        panel.SetActive(true);

        // ========================================================
        // ASEGURAR BOTONES ACTIVOS
        // ========================================================

        if (retryButton != null)
        {
            retryButton.interactable = true;
        }

        if (menuButton != null)
        {
            menuButton.interactable = true;
        }

        // ========================================================
        // INICIAR ANIMACIÓN UNA SOLA VEZ
        // ========================================================

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
        deathShown = false;
        isHiding = false;

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
                originalScale *
                startScale;
        }

        if (retryButton != null)
        {
            retryButton.interactable = true;
        }

        if (menuButton != null)
        {
            menuButton.interactable = true;
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

        // ========================================================
        // EVITAR DOBLE CIERRE
        // ========================================================

        if (isHiding)
        {
            yield break;
        }

        isHiding = true;

        // ========================================================
        // DETENER ANIMACIÓN DE APARICIÓN
        // ========================================================

        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }

        // ========================================================
        // BLOQUEAR BOTONES DURANTE EL CIERRE
        // ========================================================

        if (retryButton != null)
        {
            retryButton.interactable = false;
        }

        if (menuButton != null)
        {
            menuButton.interactable = false;
        }

        // ========================================================
        // ESTADO INICIAL
        // ========================================================

        panelCanvasGroup.alpha = 1f;

        panelRect.localScale =
            originalScale;

        float duration =
            Mathf.Max(
                0.01f,
                hideAnimationDuration
            );

        float elapsed = 0f;

        // ========================================================
        // ANIMACIÓN
        // ========================================================

        while (elapsed < duration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float smooth =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            // ----------------------------------------------------
            // DESVANECER
            // ----------------------------------------------------

            panelCanvasGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    smooth
                );

            // ----------------------------------------------------
            // ENCOGER
            // ----------------------------------------------------

            panelRect.localScale =
                Vector3.Lerp(
                    originalScale,
                    originalScale * startScale,
                    smooth
                );

            yield return null;
        }

        // ========================================================
        // ESTADO FINAL
        // ========================================================

        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;

        panel.SetActive(false);

        // ========================================================
        // RESTAURAR ESTADO PARA UNA FUTURA MUERTE
        // ========================================================

        deathShown = false;
        isHiding = false;

        animationRoutine = null;

        if (retryButton != null)
        {
            retryButton.interactable = true;
        }

        if (menuButton != null)
        {
            menuButton.interactable = true;
        }
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
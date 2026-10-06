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


    [Header("Animación de cierre")]

    [Tooltip(
        "Duración de la animación de cierre."
    )]
    [SerializeField] private float hideAnimationDuration = 0.75f;


    private RectTransform panelRect;

    private CanvasGroup panelCanvasGroup;

    private Vector3 originalScale;

    private Coroutine animationRoutine;

    private bool deathShown;

    private bool isHiding;


    public GameObject Panel =>
        panel;

    public TMP_Text FeedbackText =>
        feedbackText;

    public Button RetryButton =>
        retryButton;

    public Button MenuButton =>
        menuButton;


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


        originalScale =
            panelRect.localScale;


        panelCanvasGroup =
            panel.GetComponent<CanvasGroup>();

        if (panelCanvasGroup == null)
        {
            panelCanvasGroup =
                panel.AddComponent<CanvasGroup>();
        }


        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;

        deathShown = false;
        isHiding = false;
    }


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


        if (deathShown)
        {
            return;
        }

        if (isHiding)
        {
            return;
        }

        deathShown = true;


        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }


        panel.SetActive(true);


        if (retryButton != null)
        {
            retryButton.interactable = true;
        }

        if (menuButton != null)
        {
            menuButton.interactable = true;
        }


        animationRoutine =
            StartCoroutine(
                ShowAnimation()
            );
    }


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


        if (isHiding)
        {
            yield break;
        }

        isHiding = true;


        if (animationRoutine != null)
        {
            StopCoroutine(
                animationRoutine
            );

            animationRoutine = null;
        }


        if (retryButton != null)
        {
            retryButton.interactable = false;
        }

        if (menuButton != null)
        {
            menuButton.interactable = false;
        }


        panelCanvasGroup.alpha = 1f;

        panelRect.localScale =
            originalScale;

        float duration =
            Mathf.Max(
                0.01f,
                hideAnimationDuration
            );

        float elapsed = 0f;


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


            panelCanvasGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    smooth
                );


            panelRect.localScale =
                Vector3.Lerp(
                    originalScale,
                    originalScale * startScale,
                    smooth
                );

            yield return null;
        }


        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;

        panel.SetActive(false);


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


    private IEnumerator ShowAnimation()
    {
        float duration =
            Mathf.Max(
                0.01f,
                animationDuration
            );

        float elapsed = 0f;


        panelCanvasGroup.alpha = 0f;

        panelRect.localScale =
            originalScale *
            startScale;


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


        panelCanvasGroup.alpha = 1f;

        panelRect.localScale =
            originalScale;

        animationRoutine = null;
    }
}
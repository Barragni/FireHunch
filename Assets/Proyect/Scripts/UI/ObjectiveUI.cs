using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("Animación")]
    [SerializeField] private float introDuration = 0.8f;
    [SerializeField] private float moveDuration = 0.7f;

    [Header("Escalas")]
    [SerializeField] private float introScale = 2.2f;
    [SerializeField] private float finalScale = 0.85f;

    [Header("Posiciones")]
    [SerializeField] private Vector2 finalPosition = new Vector2(0f, -60f);

    [Header("Colores")]
    [SerializeField] private Color startColor = new Color(1f, 0.25f, 0.05f);
    [SerializeField] private Color finalColor = Color.white;

    [Header("Tiempo antes de subir")]
    [SerializeField] private float holdTime = 0.4f;

    private RectTransform rectTransform;
    private Coroutine animationCoroutine;

    private Vector2 centerPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        centerPosition = Vector2.zero;

        gameObject.SetActive(false);
    }

    public void ShowObjective(string message)
    {
        if (objectiveText == null)
        {
            Debug.LogError(
                "ObjectiveUI: No hay un TMP_Text asignado."
            );

            return;
        }

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        objectiveText.text = message;

        gameObject.SetActive(true);

        animationCoroutine =
            StartCoroutine(
                PlayObjectiveAnimation()
            );
    }

    private IEnumerator PlayObjectiveAnimation()
    {
        // ========================================================
        // ESTADO INICIAL
        // ========================================================

        rectTransform.anchoredPosition =
            centerPosition;

        rectTransform.localScale =
            Vector3.one * introScale;

        objectiveText.color =
            startColor;

        // ========================================================
        // APARICIÓN + CAMBIO DE COLOR + REDUCCIÓN
        // ========================================================

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / introDuration
                );

            // Suavizar animación
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.localScale =
                Vector3.Lerp(
                    Vector3.one * introScale,
                    Vector3.one,
                    t
                );

            objectiveText.color =
                Color.Lerp(
                    startColor,
                    finalColor,
                    t
                );

            yield return null;
        }

        rectTransform.localScale =
            Vector3.one;

        objectiveText.color =
            finalColor;

        // ========================================================
        // PEQUEÑA PAUSA
        // ========================================================

        yield return new WaitForSecondsRealtime(
            holdTime
        );

        // ========================================================
        // MOVER HACIA ARRIBA + HACER MÁS PEQUEÑO
        // ========================================================

        elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / moveDuration
                );

            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition =
                Vector2.Lerp(
                    centerPosition,
                    finalPosition,
                    t
                );

            rectTransform.localScale =
                Vector3.Lerp(
                    Vector3.one,
                    Vector3.one * finalScale,
                    t
                );

            yield return null;
        }

        // ========================================================
        // ESTADO FINAL
        // ========================================================

        rectTransform.anchoredPosition =
            finalPosition;

        rectTransform.localScale =
            Vector3.one * finalScale;

        animationCoroutine = null;
    }

    public void HideObjective()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);

            animationCoroutine = null;
        }

        gameObject.SetActive(false);
    }
}
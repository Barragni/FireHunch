using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DecisionEffectsManager : MonoBehaviour
{
    public static DecisionEffectsManager Instance;

    [Header("Overlay")]
    [SerializeField] private Image overlayImage;

    [Header("Transición")]
    [SerializeField] private float transitionDuration = 0.8f;

    [SerializeField]
    [Range(0f, 1f)]
    private float maxOverlayAlpha = 0.4f;

    private void Awake()
    {
        Instance = this;

        Color color = overlayImage.color;
        color.a = 0f;
        overlayImage.color = color;
    }

    public IEnumerator EnterDecisionMode()
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            Time.timeScale = Mathf.Lerp(
                1f,
                0f,
                t
            );

            Color color = overlayImage.color;

            color.a = Mathf.Lerp(
                0f,
                maxOverlayAlpha,
                t
            );

            overlayImage.color = color;

            yield return null;
        }

        Time.timeScale = 0f;

        Color finalColor = overlayImage.color;
        finalColor.a = maxOverlayAlpha;
        overlayImage.color = finalColor;
    }

    public IEnumerator ExitDecisionMode()
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            Time.timeScale = Mathf.Lerp(
                0f,
                1f,
                t
            );

            Color color = overlayImage.color;

            color.a = Mathf.Lerp(
                maxOverlayAlpha,
                0f,
                t
            );

            overlayImage.color = color;

            yield return null;
        }

        Time.timeScale = 1f;

        Color finalColor = overlayImage.color;
        finalColor.a = 0f;
        overlayImage.color = finalColor;
    }
}
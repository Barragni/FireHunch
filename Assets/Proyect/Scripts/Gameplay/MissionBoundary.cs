using System.Collections;
using TMPro;
using UnityEngine;

public class MissionBoundary : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] private TMP_Text signText;

    [Header("Animación")]
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        Color color = signText.color;
        color.a = 0f;
        signText.color = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        StartFade(1f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(
            FadeRoutine(targetAlpha)
        );
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        Color color = signText.color;

        float startAlpha = color.a;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            signText.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        signText.color = color;
    }
}
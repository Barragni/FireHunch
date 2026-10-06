using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirstSceneTutorialHint : MonoBehaviour
{
    [Header("Textos del tutorial")]
    [Tooltip("Puedes añadir tantos TextMeshPro como quieras.")]
    [SerializeField]
    private List<TMP_Text> tutorialTexts =
        new List<TMP_Text>();

    [Header("Tiempo")]
    [Tooltip("Tiempo que permanecen visibles antes de desaparecer.")]
    [SerializeField]
    private float visibleDuration = 4f;

    [Tooltip("Duración del fade out.")]
    [SerializeField]
    private float fadeDuration = 0.5f;


    private void Start()
    {

        if (NarrativeState.ReturningFromDeath)
        {
            gameObject.SetActive(false);
            return;
        }



        foreach (TMP_Text text in tutorialTexts)
        {
            if (text == null)
                continue;

            SetAlpha(
                text,
                1f
            );
        }

        StartCoroutine(
            ShowTutorial()
        );
    }


    private IEnumerator ShowTutorial()
    {

        yield return new WaitForSeconds(
            visibleDuration
        );



        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );

            float alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    t
                );

            foreach (TMP_Text text in tutorialTexts)
            {
                if (text == null)
                    continue;

                SetAlpha(
                    text,
                    alpha
                );
            }

            yield return null;
        }



        foreach (TMP_Text text in tutorialTexts)
        {
            if (text == null)
                continue;

            SetAlpha(
                text,
                0f
            );
        }



        gameObject.SetActive(false);
    }


    private void SetAlpha(
        TMP_Text text,
        float alpha
    )
    {
        Color color = text.color;

        color.a = alpha;

        text.color = color;
    }
}
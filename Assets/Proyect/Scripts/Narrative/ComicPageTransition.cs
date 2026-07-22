using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComicPageTransition : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private Image currentPage;
    [SerializeField] private Image nextPage;

    [Header("Texts")]
    [SerializeField] private TMP_Text currentText;
    [SerializeField] private TMP_Text nextText;

    [Header("Animation")]
    [SerializeField] private float duration = 0.35f;

    [SerializeField] private float slideDistance = 350f;

    [SerializeField] private float rotationAmount = 6f;

    [SerializeField] private float scaleAmount = 0.96f;

    private RectTransform currentRect;
    private RectTransform nextRect;

    private RectTransform currentTextRect;
    private RectTransform nextTextRect;

    private Vector2 currentStartPos;
    private Vector2 nextStartPos;

    private Vector2 currentTextStartPos;
    private Vector2 nextTextStartPos;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        currentRect = currentPage.rectTransform;
        nextRect = nextPage.rectTransform;

        currentStartPos = currentRect.anchoredPosition;
        nextStartPos = nextRect.anchoredPosition;

        if (currentText != null)
        {
            currentTextRect = currentText.rectTransform;
            currentTextStartPos = currentTextRect.anchoredPosition;
        }

        if (nextText != null)
        {
            nextTextRect = nextText.rectTransform;
            nextTextStartPos = nextTextRect.anchoredPosition;
        }

        ResetPages();
    }

    public void SetFirstPage(Sprite sprite, string text)
    {
        currentPage.sprite = sprite;
        currentPage.color = Color.white;

        nextPage.color = new Color(1f, 1f, 1f, 0f);

        if (currentText != null)
        {
            currentText.text = text;

            Color c = currentText.color;
            c.a = 1f;
            currentText.color = c;
        }

        if (nextText != null)
        {
            nextText.text = "";

            Color c = nextText.color;
            c.a = 0f;
            nextText.color = c;
        }
    }

    public IEnumerator Play(Sprite nextSprite, string nextPageText)
    {
        isPlaying = true;

        nextPage.sprite = nextSprite;

        if (nextText != null)
            nextText.text = nextPageText;

        nextRect.anchoredPosition =
            new Vector2(slideDistance, nextStartPos.y);

        nextRect.localRotation =
            Quaternion.Euler(0f, 0f, -rotationAmount);

        nextRect.localScale =
            Vector3.one * scaleAmount;

        nextPage.color =
            new Color(1f, 1f, 1f, 0f);

        if (nextTextRect != null)
        {
            nextTextRect.anchoredPosition =
                new Vector2(
                    slideDistance,
                    nextTextStartPos.y
                );
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                elapsed / duration
            );

            currentRect.anchoredPosition =
                Vector2.Lerp(
                    currentStartPos,
                    new Vector2(-slideDistance, currentStartPos.y),
                    t
                );

            nextRect.anchoredPosition =
                Vector2.Lerp(
                    new Vector2(slideDistance, nextStartPos.y),
                    nextStartPos,
                    t
                );

            if (currentTextRect != null)
            {
                currentTextRect.anchoredPosition =
                    Vector2.Lerp(
                        currentTextStartPos,
                        new Vector2(-slideDistance, currentTextStartPos.y),
                        t
                    );
            }

            if (nextTextRect != null)
            {
                nextTextRect.anchoredPosition =
                    Vector2.Lerp(
                        new Vector2(slideDistance, nextTextStartPos.y),
                        nextTextStartPos,
                        t
                    );
            }

            currentRect.localRotation =
                Quaternion.Lerp(
                    Quaternion.identity,
                    Quaternion.Euler(0f, 0f, rotationAmount),
                    t
                );

            nextRect.localRotation =
                Quaternion.Lerp(
                    Quaternion.Euler(0f, 0f, -rotationAmount),
                    Quaternion.identity,
                    t
                );

            currentRect.localScale =
                Vector3.Lerp(
                    Vector3.one,
                    Vector3.one * scaleAmount,
                    t
                );

            nextRect.localScale =
                Vector3.Lerp(
                    Vector3.one * scaleAmount,
                    Vector3.one,
                    t
                );

            Color currentColor = currentPage.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            currentPage.color = currentColor;

            Color nextColor = nextPage.color;
            nextColor.a = Mathf.Lerp(0f, 1f, t);
            nextPage.color = nextColor;

            if (currentText != null)
            {
                Color c = currentText.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                currentText.color = c;
            }

            if (nextText != null)
            {
                Color c = nextText.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                nextText.color = c;
            }

            yield return null;
        }

        currentPage.sprite = nextSprite;

        if (currentText != null)
            currentText.text = nextPageText;

        ResetPages();

        isPlaying = false;
    }

    private void ResetPages()
    {
        currentRect.anchoredPosition = currentStartPos;
        nextRect.anchoredPosition = nextStartPos;

        currentRect.localRotation = Quaternion.identity;
        nextRect.localRotation = Quaternion.identity;

        currentRect.localScale = Vector3.one;
        nextRect.localScale = Vector3.one;

        currentPage.color = Color.white;
        nextPage.color = new Color(1f, 1f, 1f, 0f);

        if (currentTextRect != null)
        {
            currentTextRect.anchoredPosition = currentTextStartPos;

            Color c = currentText.color;
            c.a = 1f;
            currentText.color = c;
        }

        if (nextTextRect != null)
        {
            nextTextRect.anchoredPosition = nextTextStartPos;

            Color c = nextText.color;
            c.a = 0f;
            nextText.color = c;
        }
    }
}
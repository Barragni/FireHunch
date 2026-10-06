using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class DialogueButtonAutoHeight : MonoBehaviour
{
    [Header("Altura del botón")]
    [SerializeField] private float oneLineHeight = 30f;
    [SerializeField] private float twoLineHeight = 42f;

    [Header("Texto")]
    [SerializeField] private TMP_Text text;

    private RectTransform rectTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();

        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        UpdateHeight();
    }

    private void Update()
    {
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        if (text == null)
            return;

        text.ForceMeshUpdate();

        int lineCount = text.textInfo.lineCount;

        float targetHeight = lineCount <= 1
            ? oneLineHeight
            : twoLineHeight;

        Vector2 size = rectTransform.sizeDelta;

        if (!Mathf.Approximately(size.y, targetHeight))
        {
            size.y = targetHeight;
            rectTransform.sizeDelta = size;
        }
    }
}
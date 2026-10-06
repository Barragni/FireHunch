using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("Retrato")]
    [SerializeField] private Image speakerPortrait;

    [Header("Textos")]
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Click para continuar")]
    [SerializeField] private GameObject continueText;

    [Header("Animación de diálogo")]
    [SerializeField] private float transitionDuration = 0.18f;

    [SerializeField] private float transitionDistance = 25f;

    [Range(0.8f, 1f)]
    [SerializeField] private float startingScale = 0.96f;


    private RectTransform dialoguePanelRect;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition;

    private Coroutine transitionCoroutine;

    private int originalSiblingIndex;

    private GameObject oldPanel;


    private void Awake()
    {
        if (dialoguePanel == null)
            return;

        dialoguePanelRect =
            dialoguePanel.GetComponent<RectTransform>();

        canvasGroup =
            dialoguePanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                dialoguePanel.AddComponent<CanvasGroup>();
        }

        originalPosition =
            dialoguePanelRect.anchoredPosition;

        originalSiblingIndex =
            dialoguePanel.transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }


    public void ShowDialogue(DialogueLine line)
    {
        if (line == null || dialoguePanel == null)
            return;


        CleanupOldPanel();


        if (!dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);

            SetDialogueContent(line);

            canvasGroup.alpha = 1f;

            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            dialoguePanelRect.anchoredPosition =
                originalPosition;

            dialoguePanelRect.localScale =
                Vector3.one;

            PlayDialogueSound();

            return;
        }


        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }

        CleanupOldPanel();


        transitionCoroutine =
            StartCoroutine(
                TransitionToNewDialogue(line)
            );
    }


    private IEnumerator TransitionToNewDialogue(
        DialogueLine newLine
    )
    {
        float duration =
            Mathf.Max(
                0.01f,
                transitionDuration
            );


        oldPanel =
            Instantiate(
                dialoguePanel,
                dialoguePanel.transform.parent
            );

        RectTransform oldRect =
            oldPanel.GetComponent<RectTransform>();

        CanvasGroup oldGroup =
            oldPanel.GetComponent<CanvasGroup>();

        if (oldGroup == null)
        {
            oldGroup =
                oldPanel.AddComponent<CanvasGroup>();
        }


        oldRect.anchorMin =
            dialoguePanelRect.anchorMin;

        oldRect.anchorMax =
            dialoguePanelRect.anchorMax;

        oldRect.pivot =
            dialoguePanelRect.pivot;

        oldRect.sizeDelta =
            dialoguePanelRect.sizeDelta;

        oldRect.anchoredPosition =
            originalPosition;

        oldRect.localScale =
            Vector3.one;

        oldGroup.alpha = 1f;


        oldGroup.blocksRaycasts = false;
        oldGroup.interactable = false;

        // La copia mantiene la posición original del diálogo.
        oldPanel.transform.SetSiblingIndex(originalSiblingIndex);


        SetDialogueContent(newLine);

        dialoguePanelRect.anchoredPosition =
            originalPosition +
            Vector2.down * transitionDistance;

        dialoguePanelRect.localScale =
            Vector3.one *
            startingScale;

        canvasGroup.alpha = 0f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        PlayDialogueSound();


        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float smoothT =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );


            oldRect.anchoredPosition =
                Vector2.Lerp(
                    originalPosition,
                    originalPosition +
                    Vector2.down *
                    transitionDistance,
                    smoothT
                );

            oldRect.localScale =
                Vector3.Lerp(
                    Vector3.one,
                    Vector3.one *
                    startingScale,
                    smoothT
                );

            oldGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    smoothT
                );


            dialoguePanelRect.anchoredPosition =
                Vector2.Lerp(
                    originalPosition +
                    Vector2.down *
                    transitionDistance,
                    originalPosition,
                    smoothT
                );

            dialoguePanelRect.localScale =
                Vector3.Lerp(
                    Vector3.one *
                    startingScale,
                    Vector3.one,
                    smoothT
                );

            canvasGroup.alpha =
                smoothT;

            yield return null;
        }


        dialoguePanelRect.anchoredPosition =
            originalPosition;

        dialoguePanelRect.localScale =
            Vector3.one;

        canvasGroup.alpha = 1f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;


        if (oldPanel != null)
        {
            Destroy(oldPanel);
            oldPanel = null;
        }

        transitionCoroutine = null;
    }


    private void SetDialogueContent(DialogueLine line)
    {
        characterNameText.text =
            line.speakerName;

        dialogueText.text =
            line.text;

        speakerPortrait.sprite =
            line.portrait;

        speakerPortrait.enabled =
            line.portrait != null;
    }


    private void PlayDialogueSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDialogueBlip();
        }
    }


    private void CleanupOldPanel()
    {
        if (oldPanel != null)
        {
            Destroy(oldPanel);
            oldPanel = null;
        }
    }


    public void HideDialogue()
    {

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }


        CleanupOldPanel();


        dialoguePanel.SetActive(false);

        dialoguePanelRect.anchoredPosition =
            originalPosition;

        dialoguePanelRect.localScale =
            Vector3.one;

        canvasGroup.alpha = 1f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }


    public void ShowContinueText()
    {
        if (continueText != null)
        {
            continueText.SetActive(true);
        }
    }

    public void HideContinueText()
    {
        if (continueText != null)
        {
            continueText.SetActive(false);
        }
    }
}
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

    // ============================================================
    // REFERENCIAS
    // ============================================================

    private RectTransform dialoguePanelRect;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition;

    private Coroutine transitionCoroutine;

    // Copia temporal utilizada durante la transición.
    // Se guarda la referencia para poder destruirla si la transición
    // se interrumpe.
    private GameObject oldPanel;

    // ============================================================
    // AWAKE
    // ============================================================

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

        // El panel principal sí puede recibir interacción
        // cuando está visible.
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    // ============================================================
    // MOSTRAR DIÁLOGO
    // ============================================================

    public void ShowDialogue(DialogueLine line)
    {
        if (line == null || dialoguePanel == null)
            return;

        // ========================================================
        // LIMPIAR COPIA ANTERIOR
        // ========================================================

        CleanupOldPanel();

        // ========================================================
        // SI NO HABÍA DIÁLOGO VISIBLE
        // ========================================================

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

        // ========================================================
        // DETENER TRANSICIÓN ANTERIOR
        // ========================================================

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }

        // Por seguridad, eliminar cualquier copia anterior.
        CleanupOldPanel();

        // ========================================================
        // NUEVA TRANSICIÓN
        // ========================================================

        transitionCoroutine =
            StartCoroutine(
                TransitionToNewDialogue(line)
            );
    }

    // ============================================================
    // TRANSICIÓN ENTRE DIÁLOGOS
    // ============================================================

    private IEnumerator TransitionToNewDialogue(
        DialogueLine newLine
    )
    {
        float duration =
            Mathf.Max(
                0.01f,
                transitionDuration
            );

        // ========================================================
        // CREAR COPIA DEL DIÁLOGO ACTUAL
        // ========================================================

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

        // ========================================================
        // CONFIGURAR COPIA
        // ========================================================

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

        // ========================================================
        // MUY IMPORTANTE:
        // LA COPIA NUNCA DEBE BLOQUEAR LOS CLICS
        // ========================================================

        oldGroup.blocksRaycasts = false;
        oldGroup.interactable = false;

        // La copia queda visualmente detrás.
        oldPanel.transform.SetAsLastSibling();

        // ========================================================
        // PREPARAR NUEVO DIÁLOGO
        // ========================================================

        SetDialogueContent(newLine);

        dialoguePanelRect.anchoredPosition =
            originalPosition +
            Vector2.down * transitionDistance;

        dialoguePanelRect.localScale =
            Vector3.one *
            startingScale;

        canvasGroup.alpha = 0f;

        // El panel nuevo queda delante de la copia.
        dialoguePanel.transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        PlayDialogueSound();

        // ========================================================
        // ANIMAR AMBOS
        // ========================================================

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            // Suavizado.
            float smoothT =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            // ----------------------------------------------------
            // PANEL VIEJO
            // ----------------------------------------------------

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

            // ----------------------------------------------------
            // PANEL NUEVO
            // ----------------------------------------------------

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

        // ========================================================
        // FINAL
        // ========================================================

        dialoguePanelRect.anchoredPosition =
            originalPosition;

        dialoguePanelRect.localScale =
            Vector3.one;

        canvasGroup.alpha = 1f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // ========================================================
        // DESTRUIR COPIA
        // ========================================================

        if (oldPanel != null)
        {
            Destroy(oldPanel);
            oldPanel = null;
        }

        transitionCoroutine = null;
    }

    // ============================================================
    // ESTABLECER CONTENIDO
    // ============================================================

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

    // ============================================================
    // SONIDO DE DIÁLOGO
    // ============================================================

    private void PlayDialogueSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDialogueBlip();
        }
    }

    // ============================================================
    // LIMPIAR COPIA DE TRANSICIÓN
    // ============================================================

    private void CleanupOldPanel()
    {
        if (oldPanel != null)
        {
            Destroy(oldPanel);
            oldPanel = null;
        }
    }

    // ============================================================
    // OCULTAR DIÁLOGO
    // ============================================================

    public void HideDialogue()
    {
        // ========================================================
        // DETENER TRANSICIÓN
        // ========================================================

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }

        // ========================================================
        // ELIMINAR COPIA QUE PODRÍA HABER QUEDADO
        // ========================================================

        CleanupOldPanel();

        // ========================================================
        // OCULTAR PANEL PRINCIPAL
        // ========================================================

        dialoguePanel.SetActive(false);

        dialoguePanelRect.anchoredPosition =
            originalPosition;

        dialoguePanelRect.localScale =
            Vector3.one;

        canvasGroup.alpha = 1f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    // ============================================================
    // TEXTO DE CONTINUAR
    // ============================================================

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
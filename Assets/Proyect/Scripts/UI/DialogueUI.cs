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

    public void ShowDialogue(DialogueLine line)
    {
        dialoguePanel.SetActive(true);

        characterNameText.text = line.speakerName;
        dialogueText.text = line.text;

        speakerPortrait.sprite = line.portrait;
        speakerPortrait.enabled = line.portrait != null;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
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
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
}
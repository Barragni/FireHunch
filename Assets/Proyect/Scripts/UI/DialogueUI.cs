using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("Elementos de texto")]
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    public void ShowDialogue(string characterName, string dialogue)
    {
        dialoguePanel.SetActive(true);

        characterNameText.text = characterName;
        dialogueText.text = dialogue;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
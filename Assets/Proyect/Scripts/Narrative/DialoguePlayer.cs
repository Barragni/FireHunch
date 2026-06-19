using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private DialogueSequence currentSequence;
    private int currentLineIndex;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void Play(DialogueSequence sequence)
    {
        currentSequence = sequence;
        currentLineIndex = 0;
        isPlaying = true;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        dialogueUI.ShowDialogue(
            currentSequence.lines[currentLineIndex]
        );
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentSequence.lines.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        isPlaying = false;

        dialogueUI.HideDialogue();
    }
}
using System;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private DialogueSequence currentSequence;

    private int currentLineIndex;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    public bool HasMoreLines
    {
        get
        {
            if (!isPlaying || currentSequence == null)
            {
                return false;
            }

            return currentLineIndex < currentSequence.lines.Count - 1;
        }
    }

    public bool IsLastLine
    {
        get
        {
            if (!isPlaying || currentSequence == null)
            {
                return true;
            }

            return currentLineIndex >= currentSequence.lines.Count - 1;
        }
    }

    public bool DialogueCompleted
    {
        get
        {
            return isPlaying && !HasMoreLines;
        }
    }

    public event Action OnDialogueFinished;

    public event Action OnAdvanceRequested;

    public void Play(DialogueSequence sequence)
    {
        currentSequence = sequence;

        currentLineIndex = 0;

        isPlaying = true;

        dialogueUI.ShowContinueText();

        ShowCurrentLine();
    }

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnAdvanceRequested?.Invoke();
        }
    }

    private void ShowCurrentLine()
    {
        if (currentSequence == null)
        {
            return;
        }

        if (
            currentLineIndex < 0 ||
            currentLineIndex >= currentSequence.lines.Count
        )
        {
            return;
        }

        dialogueUI.ShowDialogue(
            currentSequence.lines[currentLineIndex]
        );
    }

    public bool AdvanceDialogue()
    {
        if (!HasMoreLines)
        {
            return false;
        }

        currentLineIndex++;

        ShowCurrentLine();

        return true;
    }

    public void FinishDialogue()
    {
        if (!isPlaying)
        {
            return;
        }

        isPlaying = false;

        dialogueUI.HideDialogue();

        OnDialogueFinished?.Invoke();
    }
}
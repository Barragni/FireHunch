using System;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private DialogueSequence currentSequence;

    private int currentLineIndex;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    /// <summary>
    /// Indica si todavía existe otra línea después de la actual.
    /// </summary>
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

    /// <summary>
    /// Indica si actualmente ya estamos mostrando la última línea.
    /// </summary>
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

    /// <summary>
    /// Se vuelve true cuando ya no quedan más líneas por avanzar.
    /// El diálogo sigue visible hasta que NarrativeTrigger decida finalizarlo.
    /// </summary>
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

    /// <summary>
    /// Avanza una única línea.
    /// Devuelve true si realmente avanzó.
    /// Si ya estaba en la última línea simplemente devuelve false,
    /// pero NO finaliza el diálogo.
    /// </summary>
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

    /// <summary>
    /// Finaliza el diálogo únicamente cuando NarrativeTrigger
    /// determine que también terminaron los FocusPoints.
    /// </summary>
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
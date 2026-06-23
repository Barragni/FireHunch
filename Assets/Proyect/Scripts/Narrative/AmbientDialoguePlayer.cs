using System.Collections;
using UnityEngine;

public class AmbientDialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private Coroutine currentRoutine;

    public void Play(
        AmbientDialogueSequence sequence
    )
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine =
            StartCoroutine(
                PlaySequence(sequence)
            );
    }

    private IEnumerator PlaySequence(
        AmbientDialogueSequence sequence
    )
    {
        dialogueUI.HideContinueText();

        foreach (AmbientDialogueLine line in sequence.lines)
        {
            DialogueLine tempLine =
                new DialogueLine
                {
                    speakerName = line.speakerName,
                    portrait = line.portrait,
                    text = line.text
                };

            dialogueUI.ShowDialogue(
                tempLine
            );

            yield return new WaitForSeconds(
                line.duration
            );
        }

        dialogueUI.ShowContinueText();

        dialogueUI.HideDialogue();
    }
}
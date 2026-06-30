using UnityEngine;
using System.Collections;

public class NarrativeTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    [SerializeField] private DialoguePlayer dialoguePlayer;

    [SerializeField] private DialogueSequence dialogueSequence;

    [Header("Decisión")]
    [SerializeField] private DecisionPlayer decisionPlayer;

    [SerializeField] private DecisionSequence decisionSequence;

    [Header("Cámara")]
    [SerializeField] private Transform focusTarget;

    [SerializeField] private float focusDuration = 1.5f;

    private bool triggered;

    private PlayerController currentPlayer;

    private void Start()
    {
        if (NarrativeState.SkipDialogue)
        {
            NarrativeState.SkipDialogue = false;
            triggered = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        triggered = true;

        currentPlayer =
            other.GetComponent<PlayerController>();

        StartCoroutine(
            PlaySequence()
        );
    }

    private IEnumerator PlaySequence()
    {
        currentPlayer.SetMovementEnabled(false);

        yield return currentPlayer.StartCoroutine(
            currentPlayer.LookAtTarget(
                focusTarget,
                focusDuration
            )
        );

        dialoguePlayer.OnDialogueFinished +=
            HandleDialogueFinished;

        dialoguePlayer.Play(
            dialogueSequence
        );
    }

    private void HandleDialogueFinished()
    {
        dialoguePlayer.OnDialogueFinished -=
            HandleDialogueFinished;

        StartCoroutine(
            PlayDecisionSequence()
        );
    }

    private IEnumerator PlayDecisionSequence()
    {
        yield return currentPlayer.StartCoroutine(
            currentPlayer.PlayDecisionCamera()
        );

        Camera cam =
            currentPlayer.GetPlayerCamera();

        NarrativeState.SavedPlayerPosition =
            currentPlayer.transform.position;

        NarrativeState.SavedPlayerRotation =
            currentPlayer.transform.rotation;

        NarrativeState.SavedCameraPosition =
            cam.transform.position;

        NarrativeState.SavedCameraRotation =
            cam.transform.rotation;

        decisionPlayer.ShowDecision(
            decisionSequence
        );
    }
}
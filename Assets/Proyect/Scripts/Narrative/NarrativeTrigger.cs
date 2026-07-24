using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [Header("Nodo Narrativo")]
    [SerializeField] private NarrativeNode narrativeNode;

    [Header("Diálogo")]
    [SerializeField] private DialoguePlayer dialoguePlayer;

    [Header("Decisión")]
    [SerializeField] private DecisionPlayer decisionPlayer;

    [Header("Secuencia de cámara")]
    [SerializeField] private List<FocusPoint> focusPoints = new();

    private bool triggered;
    private bool waitingCamera;
    private bool sequenceFinished;

    private int currentFocusIndex;

    private PlayerController currentPlayer;
    private NarrativeRule currentRule;

    private void Start()
    {
        if (NarrativeState.SkipDialogue)
        {
            NarrativeState.SkipDialogue = false;
            triggered = true;
        }
    }

    private void OnDestroy()
    {
        if (dialoguePlayer != null)
        {
            dialoguePlayer.OnAdvanceRequested -= HandleAdvanceRequested;
            dialoguePlayer.OnDialogueFinished -= HandleDialogueFinished;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (narrativeNode == null)
        {
            Debug.LogError(
                $"NarrativeTrigger en {gameObject.name} no tiene un NarrativeNode asignado."
            );

            return;
        }

        currentRule =
            NarrativeManager.Instance.GetCurrentRule(
                narrativeNode.nodeID
            );

        if (currentRule == null)
            return;

        triggered = true;

        currentPlayer =
            other.GetComponent<PlayerController>();

        StartCoroutine(BeginSequence());
    }

    private IEnumerator BeginSequence()
    {
        currentPlayer.SetMovementEnabled(false);
        currentPlayer.SetHeadBobEnabled(false);

        // Mostrar el modelo para las escenas enfocadas.
        currentPlayer.ShowVisual();

        currentFocusIndex = 0;

        if (focusPoints.Count > 0)
        {
            yield return MoveToFocus(0);

            // El zoom sólo ocurre una vez.
            yield return currentPlayer.StartCoroutine(
                currentPlayer.PlayDialogueZoom()
            );
        }

        if (currentRule.dialogue != null)
        {
            dialoguePlayer.OnAdvanceRequested += HandleAdvanceRequested;
            dialoguePlayer.OnDialogueFinished += HandleDialogueFinished;

            dialoguePlayer.Play(currentRule.dialogue);
        }
        else
        {
            sequenceFinished = true;
            StartCoroutine(ContinueSequence());
        }
    }

    private void HandleAdvanceRequested()
    {
        if (waitingCamera)
            return;

        if (sequenceFinished)
            return;

        if (HasNextFocusPoint())
        {
            StartCoroutine(MoveNextFocus());
            return;
        }

        bool advancedDialogue =
            dialoguePlayer.AdvanceDialogue();

        if (!advancedDialogue)
        {
            sequenceFinished = true;
            dialoguePlayer.FinishDialogue();
        }
    }

    private bool HasNextFocusPoint()
    {
        return currentFocusIndex < focusPoints.Count - 1;
    }

    private IEnumerator MoveNextFocus()
    {
        waitingCamera = true;

        currentFocusIndex++;

        yield return MoveToFocus(currentFocusIndex);

        dialoguePlayer.AdvanceDialogue();

        waitingCamera = false;
    }

    private IEnumerator MoveToFocus(int index)
    {
        if (index < 0 || index >= focusPoints.Count)
            yield break;

        FocusPoint point = focusPoints[index];

        if (point == null || point.target == null)
            yield break;

        yield return currentPlayer.StartCoroutine(
            currentPlayer.LookAtTarget(
                point.target
            )
        );
    }

    private void HandleDialogueFinished()
    {
        dialoguePlayer.OnAdvanceRequested -= HandleAdvanceRequested;
        dialoguePlayer.OnDialogueFinished -= HandleDialogueFinished;

        StartCoroutine(ContinueSequence());
    }

    private IEnumerator ContinueSequence()
    {
        if (currentRule.decision != null)
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

            AudioManager.Instance.PlayDecisionOpen();

            decisionPlayer.ShowDecision(
                currentRule.decision
            );

            yield break;
        }

        // Volvemos a primera persona.
        currentPlayer.HideVisual();

        currentPlayer.SetMovementEnabled(true);
        currentPlayer.SetHeadBobEnabled(true);
    }
}
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
        {
            Debug.LogError(
                $"No existe una Rule para el nodo '{narrativeNode.nodeID}' y la ruta '{GameState.Instance.decisionPath}'."
            );

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

        currentPlayer.SetHeadBobEnabled(false);

        foreach (FocusPoint point in focusPoints)
        {
            if (point == null || point.target == null)
            {
                continue;
            }

            yield return currentPlayer.StartCoroutine(
                currentPlayer.LookAtTarget(
                    point.target
                )
            );

            yield return new WaitForSeconds(
                point.focusDuration
            );
        }

        if (currentRule.dialogue != null)
        {
            dialoguePlayer.OnDialogueFinished +=
                HandleDialogueFinished;

            dialoguePlayer.Play(
                currentRule.dialogue
            );
        }
        else
        {
            HandleDialogueFinished();
        }
    }

    private void HandleDialogueFinished()
    {
        dialoguePlayer.OnDialogueFinished -=
            HandleDialogueFinished;

        StartCoroutine(
            ContinueSequence()
        );
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

        currentPlayer.SetMovementEnabled(true);

        currentPlayer.SetHeadBobEnabled(true);
    }
}
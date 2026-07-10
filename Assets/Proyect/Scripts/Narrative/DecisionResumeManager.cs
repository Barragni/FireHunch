using System.Collections;
using UnityEngine;

public class DecisionResumeManager : MonoBehaviour
{
    [SerializeField] private DecisionPlayer decisionPlayer;

    [SerializeField] private PlayerController player;

    private DecisionSequence savedDecision;

    private void Start()
    {
        if (!NarrativeState.ReturningFromDeath)
        {
            return;
        }

        if (NarrativeState.PendingDecision == null)
        {
            return;
        }

        savedDecision =
            NarrativeState.PendingDecision;

        NarrativeState.PendingDecision = null;

        StartCoroutine(
            RestoreDecisionState()
        );
    }

    private IEnumerator RestoreDecisionState()
    {
        yield return null;

        player.transform.position =
            NarrativeState.SavedPlayerPosition;

        player.transform.rotation =
            NarrativeState.SavedPlayerRotation;

        player.SetMovementEnabled(false);

        player.SetHeadBobEnabled(false);

        yield return player.StartCoroutine(
            player.PlayDecisionCamera()
        );

        decisionPlayer.ShowDecision(
            savedDecision
        );

        NarrativeState.ReturningFromDeath = false;
    }
}
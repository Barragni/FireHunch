using UnityEngine;

public class DecisionResumeManager : MonoBehaviour
{
    [SerializeField] private DecisionPlayer decisionPlayer;

    [SerializeField] private PlayerController player;

    private DecisionSequence savedDecision;

    private void Start()
    {
        if (NarrativeState.PendingDecision == null)
        {
            return;
        }

        savedDecision =
            NarrativeState.PendingDecision;

        NarrativeState.PendingDecision = null;

        Invoke(
            nameof(RestoreDecisionState),
            0.2f
        );
    }

    private void RestoreDecisionState()
    {
        player.SetMovementEnabled(false);

        decisionPlayer.ShowDecision(
            savedDecision
        );
    }
}
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
        player.transform.position =
            NarrativeState.SavedPlayerPosition;

        player.transform.rotation =
            NarrativeState.SavedPlayerRotation;

        Camera cam =
            player.GetPlayerCamera();

        cam.transform.position =
            NarrativeState.SavedCameraPosition;

        cam.transform.rotation =
            NarrativeState.SavedCameraRotation;

        player.SetMovementEnabled(false);

        decisionPlayer.ShowDecision(
            savedDecision
        );
    }
}
using System.Collections;
using UnityEngine;

public class DecisionResumeManager : MonoBehaviour
{
    [SerializeField] private DecisionPlayer decisionPlayer;

    [SerializeField] private PlayerController player;

    private DecisionSequence savedDecision;

    private string savedNodeID;

    private void Start()
    {
        if (!NarrativeState.ReturningFromDeath)
        {
            return;
        }

        if (NarrativeState.PendingDecision == null)
        {
            Debug.LogWarning(
                "DecisionResumeManager: No existe PendingDecision."
            );

            NarrativeState.ReturningFromDeath = false;

            return;
        }

        savedDecision =
            NarrativeState.PendingDecision;

        savedNodeID =
            NarrativeState.ReturnNodeID;

        if (string.IsNullOrEmpty(savedNodeID))
        {
            if (GameState.Instance != null)
            {
                savedNodeID =
                    GameState.Instance.currentNode;
            }
        }

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
            savedDecision,
            savedNodeID
        );


        NarrativeState.ReturningFromDeath =
            false;

        NarrativeState.SkipDialogue =
            false;
    }
}
using UnityEngine;

public static class NarrativeState
{
    public static DecisionSequence PendingDecision;

    public static bool ReturningFromDeath;

    public static bool SkipDialogue;

    public static Vector3 SavedPlayerPosition;

    public static Quaternion SavedPlayerRotation;

    public static Vector3 SavedCameraPosition;

    public static Quaternion SavedCameraRotation;
}
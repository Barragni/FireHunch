using UnityEngine;

public static class NarrativeState
{
    public static DecisionSequence PendingDecision;

    public static bool ReturningFromDeath;

    public static bool SkipDialogue;

    public static string ReturnScene;

    public static string ReturnNodeID;

    public static Vector3 SavedPlayerPosition;

    public static Quaternion SavedPlayerRotation;

    public static Vector3 SavedCameraPosition;

    public static Quaternion SavedCameraRotation;

    public static AmbientDialogueSequence CurrentAmbientDialogue;

    // ============================================================
    // MÚSICA ANTES DE LA MUERTE
    // ============================================================

    /// <summary>
    /// Indica que existe una música guardada de antes de morir
    /// que debe restaurarse al pulsar Reintentar.
    /// </summary>
    public static bool RestoreMusicAfterDeath;

    /// <summary>
    /// Música que estaba sonando inmediatamente antes de la muerte.
    /// </summary>
    public static AudioClip MusicBeforeDeath;
}
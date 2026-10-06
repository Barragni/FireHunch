using UnityEngine;

public enum CompanionAnimationState
{
    Standing,
    Crouching,
    Injured
}

public class CompanionSpawnPoint : MonoBehaviour
{
    [Header("Compañero")]
    public CompanionType companionType;

    [Header("Estado de animación")]
    [Tooltip("Define si el compañero aparecerá de pie, agachado o herido.")]
    public CompanionAnimationState animationState =
        CompanionAnimationState.Standing;

    [Header("Animator Controllers")]
    [Tooltip("Animator Controller utilizado cuando el compañero está de pie.")]
    public RuntimeAnimatorController standingController;

    [Tooltip("Animator Controller utilizado cuando el compañero está agachado.")]
    public RuntimeAnimatorController crouchingController;

    [Tooltip("Animator Controller utilizado cuando el compañero está herido.")]
    public RuntimeAnimatorController injuredController;

    public RuntimeAnimatorController GetSelectedController()
    {
        switch (animationState)
        {
            case CompanionAnimationState.Crouching:
                return crouchingController;

            case CompanionAnimationState.Injured:
                return injuredController;

            case CompanionAnimationState.Standing:
            default:
                return standingController;
        }
    }
}
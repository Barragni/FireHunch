using UnityEngine;
using System.Collections;

public class NarrativeTrigger : MonoBehaviour
{
    [Header("Sistema de diálogo")]
    [SerializeField] private DialoguePlayer dialoguePlayer;

    [Header("Secuencia a reproducir")]
    [SerializeField] private DialogueSequence dialogueSequence;

    [Header("Punto que la cámara observará")]
    [SerializeField] private Transform focusTarget;

    [Header("Tiempo de transición")]
    [SerializeField] private float focusDuration = 1.5f;

    private bool triggered;

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

        StartCoroutine(
            PlaySequence(
                other.GetComponent<PlayerController>()
            )
        );
    }

    private IEnumerator PlaySequence(
        PlayerController player
    )
    {
        player.SetMovementEnabled(false);

        yield return player.StartCoroutine(
            player.LookAtTarget(
                focusTarget,
                focusDuration
            )
        );

        dialoguePlayer.Play(
            dialogueSequence
        );
    }
}
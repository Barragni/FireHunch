using UnityEngine;

public class AmbientDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private AmbientDialoguePlayer player;

    [SerializeField]
    private AmbientDialogueSequence sequence;

    private bool triggered;

    private void OnTriggerEnter(
        Collider other
    )
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

        player.Play(
            sequence
        );
    }
}
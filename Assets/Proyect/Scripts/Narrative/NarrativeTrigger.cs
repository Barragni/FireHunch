using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

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

        PlayerController player =
            other.GetComponent<PlayerController>();

        player.SetMovementEnabled(false);

        dialogueUI.ShowDialogue(
            "Marcos",
            "Algo no está bien. Debería revisar la sala de servidores."
        );
    }
}
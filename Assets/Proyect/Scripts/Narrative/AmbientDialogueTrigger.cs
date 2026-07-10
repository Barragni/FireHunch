using UnityEngine;

public class AmbientDialogueTrigger : MonoBehaviour
{
    [Header("Nodo Narrativo")]
    [SerializeField] private NarrativeNode narrativeNode;

    [Header("Player")]
    [SerializeField] private AmbientDialoguePlayer player;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (narrativeNode == null)
        {
            Debug.LogError(
                $"AmbientDialogueTrigger en {gameObject.name} no tiene un NarrativeNode asignado."
            );

            return;
        }

        NarrativeRule rule =
            NarrativeManager.Instance.GetCurrentRule(
                narrativeNode.nodeID
            );

        if (rule == null)
        {
            Debug.LogError(
                $"No existe una Rule para el nodo '{narrativeNode.nodeID}' y la ruta '{GameState.Instance.decisionPath}'."
            );

            return;
        }

        if (rule.ambientDialogue == null)
        {
            Debug.Log(
                $"La Rule del nodo '{narrativeNode.nodeID}' no tiene diálogo ambiental."
            );

            return;
        }

        triggered = true;

        player.Play(
            rule.ambientDialogue
        );
    }
}
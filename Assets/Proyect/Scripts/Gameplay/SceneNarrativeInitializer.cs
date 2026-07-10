using UnityEngine;

public class SceneNarrativeInitializer : MonoBehaviour
{
    [Header("Nodo narrativo de esta escena")]
    [SerializeField] private NarrativeNode narrativeNode;

    private void Start()
    {
        if (narrativeNode == null)
        {
            Debug.LogError(
                $"SceneNarrativeInitializer en '{gameObject.name}' no tiene un NarrativeNode asignado."
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

        if (CompanionManager.Instance != null)
        {
            CompanionManager.Instance.SpawnCompanions(
                rule.companions
            );
        }
        else
        {
            Debug.LogError(
                "No existe un CompanionManager en la escena."
            );
        }
    }
}
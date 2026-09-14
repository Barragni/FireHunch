using UnityEngine;

public class SceneObjectiveInitializer : MonoBehaviour
{
    [Header("UI del objetivo")]
    [SerializeField] private ObjectiveUI objectiveUI;

    [Header("Objetivo")]
    [TextArea(2, 4)]
    [SerializeField] private string objectiveText =
        "ENCUENTRA UNA SALIDA SEGURA";

    private void Start()
    {
        if (objectiveUI == null)
        {
            Debug.LogError(
                "SceneObjectiveInitializer: " +
                "No hay un ObjectiveUI asignado."
            );

            return;
        }

        // ========================================================
        // SI VENIMOS DE UN RETRY
        // ========================================================

        if (NarrativeState.ReturningFromDeath)
        {
            objectiveUI.ShowObjectiveFinal(
                objectiveText
            );

            return;
        }

        // ========================================================
        // ENTRADA NORMAL
        // ========================================================

        objectiveUI.ShowObjective(
            objectiveText
        );
    }
}
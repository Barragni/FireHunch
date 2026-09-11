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

        objectiveUI.ShowObjective(
            objectiveText
        );
    }
}
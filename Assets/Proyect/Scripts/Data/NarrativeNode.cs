using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewNarrativeNode",
    menuName = "Narrative/Narrative Node"
)]
public class NarrativeNode : ScriptableObject
{
    [Header("Identificador único")]
    public string nodeID;

    // ============================================================
    // REVISIÓN DE DECISIÓN
    // ============================================================

    [Header("Revisión de la decisión")]

    [Tooltip(
        "Imagen que se mostrará al revisar la decisión correspondiente a este escenario."
    )]
    public Sprite decisionReviewImage;

    [TextArea(3, 8)]
    [Tooltip(
        "Retroalimentación general de la decisión de este escenario. " +
        "Es la misma independientemente de la opción seleccionada."
    )]
    public string decisionFeedback;

    // ============================================================
    // REGLAS
    // ============================================================

    [Header("Reglas")]
    public List<NarrativeRule> rules = new();

    public NarrativeRule GetRule(string route)
    {
        foreach (NarrativeRule rule in rules)
        {
            if (rule == null)
                continue;

            if (rule.route == route)
            {
                return rule;
            }
        }

        return null;
    }
}
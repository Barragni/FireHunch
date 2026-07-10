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

    [Header("Reglas")]
    public List<NarrativeRule> rules = new();

    public NarrativeRule GetRule(string route)
    {
        foreach (NarrativeRule rule in rules)
        {
            if (rule.route == route)
            {
                return rule;
            }
        }

        return null;
    }
}
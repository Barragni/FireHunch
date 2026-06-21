using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDecisionSequence",
    menuName = "Narrative/Decision Sequence"
)]
public class DecisionSequence : ScriptableObject
{
    [Header("Pregunta mostrada")]
    [TextArea(2, 4)]
    public string question;

    [Header("Opciones disponibles")]
    public List<DecisionOption> options = new();
}
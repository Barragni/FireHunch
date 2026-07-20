using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDecisionSequence",
    menuName = "Narrative/Decision Sequence"
)]
public class DecisionSequence : ScriptableObject
{
    [Header("Identificación")]
    [Tooltip("Debe ser único. Ejemplo: Decision_01, Decision_02, Decision_Final...")]
    public string decisionID;

    [Header("Pregunta mostrada")]
    [TextArea(2, 4)]
    public string question;

    [Header("Tiempo límite (segundos)")]
    public float timeLimit = 10f;

    [Header("Resultado al agotarse el tiempo")]
    [Tooltip("Normalmente será Death. Para decisiones especiales posteriores a un cómic puede configurarse como Incorrect.")]
    public DecisionOutcomeType timeoutResult = DecisionOutcomeType.Death;

    [Header("Opciones disponibles")]
    public List<DecisionOption> options = new();
}
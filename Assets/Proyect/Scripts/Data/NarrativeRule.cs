using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeRule
{
    [Header("Ruta de decisiones")]
    [Tooltip("Ejemplo: CLCM")]
    public string route;

    [Header("Escena destino")]
    public string sceneName;

    [Header("Diálogo principal")]
    public DialogueSequence dialogue;

    [Header("Diálogo ambiental")]
    public AmbientDialogueSequence ambientDialogue;

    [Header("Decisión")]
    public DecisionSequence decision;

    [Header("Personajes que deben aparecer")]
    public List<CompanionData> companions = new();
}
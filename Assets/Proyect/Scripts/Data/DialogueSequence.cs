using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDialogueSequence",
    menuName = "Narrative/Dialogue Sequence"
)]
public class DialogueSequence : ScriptableObject
{
    [Header("Bloquear jugador mientras habla")]
    public bool lockPlayer = true;

    [Header("Líneas del diálogo")]
    public List<DialogueLine> lines = new();
}
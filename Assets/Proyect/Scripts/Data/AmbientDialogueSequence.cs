using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewAmbientDialogue",
    menuName = "Narrative/Ambient Dialogue"
)]
public class AmbientDialogueSequence : ScriptableObject
{
    public List<AmbientDialogueLine> lines = new();
}
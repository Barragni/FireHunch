using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NarrativeDatabase",
    menuName = "Narrative/Narrative Database"
)]
public class NarrativeDatabase : ScriptableObject
{
    public List<NarrativeNode> nodes = new();
}
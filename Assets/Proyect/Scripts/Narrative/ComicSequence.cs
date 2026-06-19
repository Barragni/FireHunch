using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewComicSequence",
    menuName = "Narrative/Comic Sequence"
)]
public class ComicSequence : ScriptableObject
{
    [Header("Lista de viñetas que componen la secuencia")]
    public List<ComicPanelData> panels = new();
}
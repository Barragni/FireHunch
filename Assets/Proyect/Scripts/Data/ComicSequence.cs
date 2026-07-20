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

    [Header("Decisión opcional al finalizar")]
    [Tooltip("Si se asigna una DecisionSequence, al terminar la última viñeta aparecerá inmediatamente el panel de decisión en lugar de continuar con el flujo normal.")]
    public DecisionSequence decisionAfterComic;
}
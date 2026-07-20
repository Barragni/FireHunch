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
    [Tooltip("Si se asigna una DecisionSequence, al terminar la última viñeta aparecerá inmediatamente el panel de decisión.")]
    public DecisionSequence decisionAfterComic;

    [Header("Comic final opcional")]
    [Tooltip("Si se asigna, este comic se reproducirá inmediatamente al terminar esta secuencia.")]
    public ComicSequence endingComic;

    [Header("Finaliza el juego")]
    [Tooltip("Si está activado, al terminar este comic se cargará la escena Results.")]
    public bool endsWithResults;
}
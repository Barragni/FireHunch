using UnityEngine;

[System.Serializable]
public class DecisionOption
{
    [Header("Texto mostrado")]
    public string optionText;

    [Header("Puntaje")]
    public int score;

    [Header("Tipo de resultado")]
    public DecisionOutcomeType outcomeType;

    [Header("Comic a reproducir")]
    public ComicSequence comicSequence;

    [Header("Escena destino")]
    public string nextScene;

    [Header("Retroalimentación de muerte")]
    [TextArea(3, 8)]
    public string deathFeedback;
}
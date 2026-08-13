using UnityEngine;

[System.Serializable]
public class DecisionOption
{
    [Header("Texto mostrado")]
    public string optionText;

    [Header("Tipo de resultado")]
    public DecisionOutcomeType outcomeType;

    [Header("Comic a reproducir")]
    public ComicSequence comicSequence;

    [Header("Nodo narrativo siguiente")]
    public string nextNode;

    [Header("Retroalimentación de muerte")]
    [TextArea(3, 8)]
    public string deathFeedback;

    [Header("Imagen para revisión")]
    public Sprite reviewImage;

    [Header("Retroalimentación para revisión")]
    [TextArea(3, 8)]
    public string reviewFeedback;
}
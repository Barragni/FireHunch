using UnityEngine;

[System.Serializable]
public class AmbientDialogueLine
{
    [Header("Nombre")]
    public string speakerName;

    [Header("Retrato")]
    public Sprite portrait;

    [Header("Texto")]
    [TextArea(3, 6)]
    public string text;

    [Header("Tiempo en pantalla")]
    public float duration = 3f;
}
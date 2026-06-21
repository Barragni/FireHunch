using UnityEngine;

[System.Serializable]
public class ComicPanelData
{
    [Header("Texto que aparecerá en esta viñeta")]
    [TextArea(3, 6)]
    public string text;

    [Header("Imagen de la viñeta")]
    public Sprite image;

    [Header("Duración opcional para futuras reproducciones automáticas")]
    public float duration = 3f;
}
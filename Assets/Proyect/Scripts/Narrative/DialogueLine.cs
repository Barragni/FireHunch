using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("Nombre del personaje")]
    public string speakerName;

    [Header("Retrato del personaje")]
    public Sprite portrait;

    [Header("Texto del diálogo")]
    [TextArea(3, 6)]
    public string text;
}
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCompanion",
    menuName = "Narrative/Companion"
)]
public class CompanionData : ScriptableObject
{
    public CompanionType companionType;

    public GameObject prefab;
}
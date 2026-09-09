using UnityEngine;

[CreateAssetMenu(
    fileName = "NewRescueCharacter",
    menuName = "Narrative/Rescue Character"
)]
public class RescueCharacterData : ScriptableObject
{
    [Header("Personaje")]
    public CompanionType characterType;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Animación de herido")]
    public RuntimeAnimatorController injuredController;
}
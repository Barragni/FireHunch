using UnityEngine;

[CreateAssetMenu(
    fileName = "NewRouteCharacter",
    menuName = "Narrative/Route Character"
)]
public class RouteCharacterData : ScriptableObject
{
    [Header("Prefab del personaje")]
    public GameObject prefab;
}
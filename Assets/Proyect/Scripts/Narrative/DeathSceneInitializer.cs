using UnityEngine;

public class DeathSceneInitializer : MonoBehaviour
{
    [SerializeField] private DeathPlayer deathPlayer;

    private void Start()
    {
        deathPlayer.ShowDeath(
            DecisionState.SelectedOption.deathFeedback
        );
    }
}
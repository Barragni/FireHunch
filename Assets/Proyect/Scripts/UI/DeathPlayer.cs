using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPlayer : MonoBehaviour
{
    [SerializeField] private DeathUI deathUI;

    private void Start()
    {
        deathUI.DeathPanel.SetActive(false);

        deathUI.RetryButton.onClick.AddListener(
            Retry
        );

        deathUI.MainMenuButton.onClick.AddListener(
            ReturnToMenu
        );

        ShowDeath(
            DecisionState.SelectedOption.deathFeedback
        );
    }

    public void ShowDeath(
        string feedback
    )
    {
        deathUI.DeathPanel.SetActive(true);

        deathUI.FeedbackText.text =
            feedback;
    }

    private void Retry()
    {
        SceneManager.LoadScene(
            "OfficeFloor"
        );
    }

    private void ReturnToMenu()
    {
        GameState.Instance.ResetData();

        SceneManager.LoadScene(
            "Menu"
        );
    }
}
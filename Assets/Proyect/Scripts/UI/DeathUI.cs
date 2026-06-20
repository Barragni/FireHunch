using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;

    [SerializeField] private TMP_Text feedbackText;

    [SerializeField] private Button retryButton;

    [SerializeField] private Button mainMenuButton;

    public GameObject DeathPanel => deathPanel;

    public TMP_Text FeedbackText => feedbackText;

    public Button RetryButton => retryButton;

    public Button MainMenuButton => mainMenuButton;
}
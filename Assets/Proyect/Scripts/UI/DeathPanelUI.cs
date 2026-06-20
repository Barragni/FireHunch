using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    public GameObject Panel => panel;
    public TMP_Text FeedbackText => feedbackText;
    public Button RetryButton => retryButton;
    public Button MenuButton => menuButton;
}
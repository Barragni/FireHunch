using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("Panel del tutorial")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Botón volver")]
    [SerializeField] private Button backButton;

    private void Awake()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(CloseTutorial);
        }
    }

    public void OpenTutorial()
    {
        if (tutorialPanel == null)
        {
            Debug.LogError(
                "TutorialUI: No se ha asignado Tutorial Panel."
            );

            return;
        }

        tutorialPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
}
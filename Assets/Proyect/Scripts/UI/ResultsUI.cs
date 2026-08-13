using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button reviewButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button creditsButton;

    [Header("Revisión de decisiones")]
    [SerializeField] private DecisionReviewUI decisionReviewUI;

    private void Awake()
    {
        if (reviewButton != null)
        {
            reviewButton.onClick.RemoveListener(OpenReview);
            reviewButton.onClick.AddListener(OpenReview);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(ReturnToMenu);
            menuButton.onClick.AddListener(ReturnToMenu);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.RemoveListener(OpenCredits);
            creditsButton.onClick.AddListener(OpenCredits);
        }
    }

    private void OnDestroy()
    {
        if (reviewButton != null)
        {
            reviewButton.onClick.RemoveListener(OpenReview);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(ReturnToMenu);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.RemoveListener(OpenCredits);
        }
    }

    private void OpenReview()
    {
        Debug.Log("ResultsUI: botón Revisar decisiones presionado.");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (decisionReviewUI == null)
        {
            Debug.LogError(
                "ResultsUI: DecisionReviewUI NO está asignado en el Inspector."
            );

            return;
        }

        Debug.Log(
            "ResultsUI: llamando a DecisionReviewUI.OpenReview()."
        );

        decisionReviewUI.OpenReview();
    }

    private void ReturnToMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        SceneTransitionManager.Instance.LoadScene(
            "Menu"
        );
    }

    private void OpenCredits()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Debug.Log("Créditos aún no implementados.");
    }
}
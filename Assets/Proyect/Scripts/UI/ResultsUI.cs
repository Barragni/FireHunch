using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button reviewButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button creditsButton;

    private void Start()
    {
        if (reviewButton != null)
            reviewButton.onClick.AddListener(OpenReview);

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OpenCredits);
    }

    private void OpenReview()
    {
        Debug.Log("Review aún no implementado.");
    }

    private void ReturnToMenu()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetData();
        }

        SceneTransitionManager.Instance.LoadScene("Menu");
    }

    private void OpenCredits()
    {
        Debug.Log("Créditos aún no implementados.");
    }
}
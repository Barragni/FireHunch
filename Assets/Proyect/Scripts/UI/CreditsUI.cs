using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Botón cerrar")]
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseCredits);
            closeButton.onClick.AddListener(CloseCredits);
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseCredits);
        }
    }

    public void OpenCredits()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }
}
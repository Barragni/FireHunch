using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;

    public void TogglePauseUI(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
    }
}
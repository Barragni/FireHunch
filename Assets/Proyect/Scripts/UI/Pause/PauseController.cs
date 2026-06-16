using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private PauseModel model;
    [SerializeField] private PauseView view;

    private void Start()
    {
        view.resumeButton.onClick.AddListener(ResumeGame);
        view.mainMenuButton.onClick.AddListener(GoToMainMenu);
        view.quitButton.onClick.AddListener(QuitGame);

        ForceResume();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        model.isPaused = !model.isPaused;
        
        
        Time.timeScale = model.isPaused ? 0f : 1f;
        
        view.TogglePauseUI(model.isPaused);
    }

    public void ResumeGame()
    {
        TogglePause(); 
    }

    private void ForceResume()
    {
        model.isPaused = false;
        Time.timeScale = 1f;
        view.TogglePauseUI(false);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("menu");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
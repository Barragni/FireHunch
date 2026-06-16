using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuModel model;
    [SerializeField] private MainMenuView view;

    private void Start()
    {
        // Vinculamos los eventos de los botones con las funciones de este script
        view.playButton.onClick.AddListener(PlayGame);
        view.optionsButton.onClick.AddListener(ToggleOptions);
        view.quitButton.onClick.AddListener(QuitGame);

        // Estado inicial de la interfaz
        view.SetOptionsPanelActive(model.isOptionsOpen);
    }

    private void PlayGame()
    {
        // Carga la escena de nivel usando su nombre exacto
        SceneManager.LoadScene("level1");
    }

    private void ToggleOptions()
    {
        model.isOptionsOpen = !model.isOptionsOpen;
        view.SetOptionsPanelActive(model.isOptionsOpen);
    }

    private void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esto funciona en la versión compilada (.exe)
    }
}
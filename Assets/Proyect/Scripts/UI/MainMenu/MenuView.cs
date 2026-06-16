using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [Header("Botones principales")]
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Paneles")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    // La vista solo se encarga de prender y apagar cosas visualmente
    public void SetOptionsPanelActive(bool isActive)
    {
        optionsPanel.SetActive(isActive);
        mainMenuPanel.SetActive(!isActive); // Oculta el principal si opciones está abierto
    }
}
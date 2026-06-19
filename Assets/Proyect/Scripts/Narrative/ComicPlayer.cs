using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicPlayer : MonoBehaviour
{
    [Header("Secuencia de viñetas")]
    [SerializeField] private ComicSequence sequence;

    [Header("Elementos de la interfaz")]
    [SerializeField] private Image panelImage;
    [SerializeField] private TMP_Text panelText;

    [Header("Escena a cargar al finalizar la secuencia")]
    [SerializeField] private string nextScene;

    private int currentPanel = 0;

    private void Start()
    {
        if (sequence == null)
        {
            Debug.LogError("No se asignó ninguna secuencia de viñetas.");
            return;
        }

        if (sequence.panels.Count == 0)
        {
            Debug.LogError("La secuencia no contiene viñetas.");
            return;
        }

        ShowCurrentPanel();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextPanel();
        }
    }

    private void ShowCurrentPanel()
    {
        panelImage.sprite = sequence.panels[currentPanel].image;
        panelText.text = sequence.panels[currentPanel].text;
    }

    private void NextPanel()
    {
        currentPanel++;

        if (currentPanel >= sequence.panels.Count)
        {
            EndSequence();
            return;
        }

        ShowCurrentPanel();
    }

    private void EndSequence()
    {
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("No se configuró una escena de destino.");
        }
    }
}
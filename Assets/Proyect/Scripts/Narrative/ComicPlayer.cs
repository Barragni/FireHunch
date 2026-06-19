using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComicPlayer : MonoBehaviour
{
    [Header("Secuencia de viñetas")]
    [SerializeField] private ComicSequence sequence;

    [Header("Referencias UI")]
    [SerializeField] private Image panelImage;
    [SerializeField] private TMP_Text panelText;

    private int currentPanel = 0;

    private void Start()
    {
        if (sequence == null)
        {
            Debug.LogError("No se asignó ninguna secuencia.");
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
            Debug.Log("Fin de la secuencia.");
            return;
        }

        ShowCurrentPanel();
    }
}
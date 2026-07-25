using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParticipantInfoUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField ageInput;

    [SerializeField] private Toggle consentToggle;

    [SerializeField] private Button startButton;

    [SerializeField] private TMP_Text validationText;

    [Header("Escena siguiente")]
    [SerializeField] private string nextScene = "Menu";

    private void Start()
    {
        validationText.gameObject.SetActive(false);

        startButton.onClick.AddListener(OnStartClicked);

        ageInput.onValueChanged.AddListener(OnInputChanged);
        consentToggle.onValueChanged.AddListener(OnConsentChanged);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartClicked);

        ageInput.onValueChanged.RemoveListener(OnInputChanged);
        consentToggle.onValueChanged.RemoveListener(OnConsentChanged);
    }

    private void OnInputChanged(string value)
    {
        HideValidation();

        // Solo permitir números.
        string filtered = "";

        foreach (char c in value)
        {
            if (char.IsDigit(c))
            {
                filtered += c;
            }
        }

        if (filtered != value)
        {
            ageInput.SetTextWithoutNotify(filtered);
            ageInput.caretPosition = filtered.Length;
        }
    }

    private void OnConsentChanged(bool value)
    {
        HideValidation();
    }

    private void OnStartClicked()
    {
        HideValidation();

        if (!int.TryParse(ageInput.text, out int age))
        {
            ShowValidation("Por favor ingresa una edad válida.");
            return;
        }

        if (age < 18 || age > 35)
        {
            ShowValidation("La edad debe estar entre 18 y 35 años.");
            return;
        }

        if (!consentToggle.isOn)
        {
            ShowValidation("Debes aceptar el consentimiento para participar.");
            return;
        }

        if (AttemptSession.Instance == null)
        {
            Debug.LogError("No existe AttemptSession.");
            return;
        }

        AttemptSession.Instance.StartSession(
            age,
            consentToggle.isOn);

        SceneManager.LoadScene(nextScene);
    }

    private void ShowValidation(string message)
    {
        validationText.text = message;
        validationText.gameObject.SetActive(true);
    }

    private void HideValidation()
    {
        validationText.text = "";
        validationText.gameObject.SetActive(false);
    }
}
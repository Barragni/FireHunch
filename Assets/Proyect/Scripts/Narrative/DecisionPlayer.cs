using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecisionPlayer : MonoBehaviour
{
    [SerializeField] private DecisionUI decisionUI;

    private DecisionSequence currentDecision;

    private void Start()
    {
        decisionUI.DecisionPanel.SetActive(false);
    }

    public void ShowDecision(
        DecisionSequence decision
    )
    {
        currentDecision = decision;

        DecisionState.CurrentDecision = decision;

        decisionUI.DecisionPanel.SetActive(true);

        decisionUI.QuestionText.text =
            decision.question;

        SetupButton(
            decisionUI.OptionButton1,
            decisionUI.OptionText1,
            0
        );

        SetupButton(
            decisionUI.OptionButton2,
            decisionUI.OptionText2,
            1
        );

        SetupButton(
            decisionUI.OptionButton3,
            decisionUI.OptionText3,
            2
        );
    }

    private void SetupButton(
        Button button,
        TMP_Text text,
        int index
    )
    {
        if (index >= currentDecision.options.Count)
        {
            button.gameObject.SetActive(false);
            return;
        }

        button.gameObject.SetActive(true);

        DecisionOption option =
            currentDecision.options[index];

        text.text = option.optionText;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            () => SelectOption(option)
        );
    }

    private void SelectOption(
        DecisionOption option
    )
    {
        DecisionState.SelectedOption = option;

        NarrativeState.PendingDecision =
            currentDecision;

        GameState.Instance.AddScore(
            option.score
        );

        decisionUI.DecisionPanel.SetActive(false);

        SceneManager.LoadScene(
            "DecisionComic"
        );
    }
}
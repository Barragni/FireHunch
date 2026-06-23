using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecisionPlayer : MonoBehaviour
{
    [SerializeField] private DecisionUI decisionUI;

    private DecisionSequence currentDecision;

    private Coroutine timerRoutine;

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

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }

        timerRoutine =
            StartCoroutine(DecisionTimer());
    }

    private IEnumerator DecisionTimer()
    {
        float timeRemaining =
            currentDecision.timeLimit;

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            decisionUI.TimerFillImage.fillAmount =
                timeRemaining /
                currentDecision.timeLimit;

            yield return null;
        }

        SelectDeathOption();
    }

    private void SelectDeathOption()
    {
        foreach (DecisionOption option in currentDecision.options)
        {
            if (
                option.outcomeType ==
                DecisionOutcomeType.Death
            )
            {
                SelectOption(option);
                return;
            }
        }

        Debug.LogWarning(
            "No existe opción Death en esta decisión."
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
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }

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
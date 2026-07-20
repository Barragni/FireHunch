using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DecisionPlayer : MonoBehaviour
{
    [SerializeField] private DecisionUI decisionUI;

    private DecisionSequence currentDecision;

    private Coroutine timerRoutine;

    public bool IsDecisionOpen =>
        decisionUI != null &&
        decisionUI.DecisionPanel.activeSelf;

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

        decisionUI.TimerFillImage.fillAmount = 1f;

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
        }

        timerRoutine =
            StartCoroutine(
                DecisionTimer()
            );
    }

    private IEnumerator DecisionTimer()
    {
        float timeRemaining =
            currentDecision.timeLimit;

        while (timeRemaining > 0f)
        {
            timeRemaining -=
                Time.unscaledDeltaTime;

            decisionUI.TimerFillImage.fillAmount =
                Mathf.Clamp01(
                    timeRemaining /
                    currentDecision.timeLimit
                );

            yield return null;
        }

        SelectTimeoutOption();
    }

    /// <summary>
    /// Selecciona automáticamente la opción definida
    /// por DecisionSequence.timeoutResult.
    /// </summary>
    private void SelectTimeoutOption()
    {
        AudioManager.Instance.PlayTimerEnd();

        foreach (
            DecisionOption option
            in currentDecision.options
        )
        {
            if (
                option.outcomeType ==
                currentDecision.timeoutResult
            )
            {
                StartCoroutine(
                    SelectOption(option)
                );

                return;
            }
        }

        Debug.LogWarning(
            $"No existe una opción de tipo {currentDecision.timeoutResult} en esta DecisionSequence."
        );
    }

    private void SetupButton(
        Button button,
        TMP_Text text,
        int index
    )
    {
        if (
            index >=
            currentDecision.options.Count
        )
        {
            button.gameObject.SetActive(false);
            return;
        }

        button.gameObject.SetActive(true);

        DecisionOption option =
            currentDecision.options[index];

        text.text =
            option.optionText;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            () =>
            {
                StartCoroutine(
                    SelectOption(option)
                );
            }
        );
    }

    private IEnumerator SelectOption(
        DecisionOption option
    )
    {
        AudioManager.Instance.PlayButtonClick();

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        decisionUI.DecisionPanel.SetActive(false);

        DecisionState.SelectedOption =
            option;

        NarrativeState.PendingDecision =
            currentDecision;

        NarrativeState.ReturnScene =
            SceneManager.GetActiveScene().name;

        GameState.Instance.AddScore(
            option.score
        );

        switch (option.outcomeType)
        {
            case DecisionOutcomeType.Correct:
                GameState.Instance.RegisterDecision("C");
                break;

            case DecisionOutcomeType.Incorrect:
                GameState.Instance.RegisterDecision("L");
                break;

            case DecisionOutcomeType.Death:
                // La muerte no modifica la ruta narrativa.
                break;
        }

        SceneTransitionManager.Instance.LoadScene(
            "DecisionComic"
        );

        yield break;
    }
}
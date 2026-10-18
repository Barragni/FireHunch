using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DecisionPlayer : MonoBehaviour
{
    [SerializeField] private DecisionUI decisionUI;

    [Header("Controles móviles")]
    [SerializeField] private GameObject mobileJoystick;
    [SerializeField] private GameObject mobileLookJoystick;
    [SerializeField] private GameObject mobilePauseButton;

    private DecisionSequence currentDecision;
    private string currentNodeID = "";

    private Coroutine timerRoutine;

    public bool IsDecisionOpen =>
        decisionUI != null &&
        decisionUI.DecisionPanel.activeSelf;


    private void Start()
    {
        decisionUI.DecisionPanel.SetActive(false);

        SetMobileControlsVisible(true);
    }



    public void ShowDecision(
        DecisionSequence decision
    )
    {
        string nodeID = "";

        if (GameState.Instance != null)
        {
            nodeID =
                GameState.Instance.currentNode;
        }

        ShowDecision(
            decision,
            nodeID
        );
    }



    public void ShowDecision(
        DecisionSequence decision,
        string nodeID
    )
    {
        if (decision == null)
        {
            Debug.LogError(
                "DecisionPlayer: Se intentó mostrar una DecisionSequence NULL."
            );

            return;
        }

        currentDecision = decision;

        currentNodeID = nodeID;

        DecisionState.CurrentDecision =
            decision;

        decisionUI.DecisionPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetMobileControlsVisible(false);

        if (GameState.Instance != null)
        {
            GameState.Instance.BeginDecisionAttempt(
                currentDecision.decisionID,
                currentNodeID
            );
        }

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

        decisionUI.TimerFillImage.fillAmount =
            1f;

        if (timerRoutine != null)
        {
            StopCoroutine(
                timerRoutine
            );
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

        timerRoutine = null;

        SelectTimeoutOption();
    }


    private void SelectTimeoutOption()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTimerEnd();
        }

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
            $"No existe una opción de tipo " +
            $"{currentDecision.timeoutResult}."
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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (timerRoutine != null)
        {
            StopCoroutine(
                timerRoutine
            );

            timerRoutine = null;
        }


        DecisionState.SelectedOption =
            option;

        NarrativeState.PendingDecision =
            currentDecision;

        NarrativeState.ReturnScene =
            SceneManager.GetActiveScene().name;

        NarrativeState.ReturnNodeID =
            currentNodeID;


        decisionUI.DecisionPanel.SetActive(false);


        SetMobileControlsVisible(true);


        if (
            DeviceModeManager.Instance != null &&
            !DeviceModeManager.Instance.IsMobile
        )
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        int selectedOptionIndex =
            currentDecision.options.IndexOf(
                option
            );


        if (GameState.Instance != null)
        {
            GameState.Instance.RegisterDecisionAttempt(
                currentDecision.decisionID,
                currentNodeID,
                selectedOptionIndex,
                option.outcomeType
            );
        }


        switch (option.outcomeType)
        {
            case DecisionOutcomeType.Correct:

                GameState.Instance.RegisterFinalDecision(
                    currentDecision.decisionID,
                    currentNodeID,
                    selectedOptionIndex,
                    DecisionOutcomeType.Correct,
                    false
                );

                GameState.Instance.RegisterDecision(
                    "C"
                );

                break;


            case DecisionOutcomeType.Incorrect:

                GameState.Instance.RegisterFinalDecision(
                    currentDecision.decisionID,
                    currentNodeID,
                    selectedOptionIndex,
                    DecisionOutcomeType.Incorrect,
                    false
                );

                GameState.Instance.RegisterDecision(
                    "L"
                );

                break;


            case DecisionOutcomeType.Death:


                GameState.Instance.RegisterDeath(
                    currentDecision.decisionID,
                    currentNodeID
                );

                break;
        }


        ComicState.CurrentSequence =
            option.comicSequence;

        ComicState.IsIntro =
            false;

        ComicState.IntroNextScene =
            string.Empty;

        SceneTransitionManager.Instance.LoadScene(
            "DecisionComic"
        );

        yield break;
    }



    private void SetMobileControlsVisible(bool visible)
    {
        bool isMobileDevice =
            DeviceModeManager.Instance != null &&
            DeviceModeManager.Instance.IsMobile;

        bool shouldBeVisible =
            visible &&
            isMobileDevice;

        if (mobileJoystick != null)
        {
            mobileJoystick.SetActive(
                shouldBeVisible
            );
        }

        if (mobileLookJoystick != null)
        {
            mobileLookJoystick.SetActive(
                shouldBeVisible
            );
        }

        if (mobilePauseButton != null)
        {
            mobilePauseButton.SetActive(
                shouldBeVisible
            );
        }
    }
}
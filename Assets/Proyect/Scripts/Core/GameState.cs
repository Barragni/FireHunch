using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [Header("Datos generales")]
    public int score = 0;
    public bool hasValentina = false;
    public bool hasBeto = false;

    [Header("Progreso narrativo")]
    public string currentNode = "";

    [Header("Ruta de decisiones")]
    public string decisionPath = "";

    [Header("Historial de decisiones")]
    public List<string> decisionHistory = new();

    [Header("Registros de decisiones")]
    public List<DecisionRecord> decisionRecords = new();

    [Header("Contadores")]
    public int CorrectChoices { get; private set; }
    public int IncorrectChoices { get; private set; }
    public int DeathChoices { get; private set; }


    private string activeDecisionID = "";

    private float activeDecisionStartTime;

    private int activeDecisionAttemptNumber;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void AddScore(int amount)
    {
        score += amount;
    }


    public void BeginDecisionAttempt(
        string decisionID,
        string nodeID
    )
    {
        if (string.IsNullOrEmpty(decisionID))
        {
            Debug.LogWarning(
                "GameState: Se intentó comenzar una decisión sin decisionID."
            );

            return;
        }

        activeDecisionID = decisionID;

        activeDecisionStartTime =
            Time.unscaledTime;

        DecisionRecord record =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );

        activeDecisionAttemptNumber =
            record.attempts.Count + 1;
    }


    public void RegisterDecisionAttempt(
        string decisionID,
        string nodeID,
        int optionIndex,
        DecisionOutcomeType outcome
    )
    {
        if (string.IsNullOrEmpty(decisionID))
        {
            Debug.LogWarning(
                "GameState: No se puede registrar un intento sin decisionID."
            );

            return;
        }



        DecisionRecord record =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );



        float elapsedTime = 0f;

        if (activeDecisionID == decisionID)
        {
            elapsedTime =
                Time.unscaledTime -
                activeDecisionStartTime;
        }

        elapsedTime =
            Mathf.Max(
                0f,
                elapsedTime
            );



        int attemptNumber =
            record.attempts.Count + 1;



        DecisionAttemptRecord attempt =
            new DecisionAttemptRecord
            {
                attemptNumber =
                    attemptNumber,

                selectedOptionIndex =
                    optionIndex,

                outcome =
                    outcome,

                timeSeconds =
                    elapsedTime
            };


        record.attempts.Add(
            attempt
        );



        record.totalTimeSeconds +=
            elapsedTime;


        if (outcome == DecisionOutcomeType.Death)
        {
            record.retryCount++;

            record.diedAtLeastOnce =
                true;
        }



        activeDecisionID = "";

        activeDecisionStartTime = 0f;

        activeDecisionAttemptNumber = 0;
    }



    public void RegisterDecision(
        string decisionID,
        int optionIndex
    )
    {
        RegisterDecision(
            decisionID,
            "",
            optionIndex,
            DecisionOutcomeType.Incorrect,
            false
        );
    }


    public void RegisterDecision(string result)
    {
        if (string.IsNullOrEmpty(result))
        {
            return;
        }

        switch (result)
        {
            case "C":

                decisionPath += "C";
                CorrectChoices++;

                break;


            case "L":

                decisionPath += "L";
                IncorrectChoices++;

                break;


            case "M":

                DeathChoices++;

                break;


            default:

                Debug.LogWarning(
                    $"GameState: Resultado de decisión desconocido '{result}'."
                );

                break;
        }
    }



    public void RegisterDecision(
        string decisionID,
        string nodeID,
        int optionIndex,
        DecisionOutcomeType outcome,
        bool diedAtLeastOnce
    )
    {
        decisionHistory.Add(
            decisionID
        );


        switch (outcome)
        {
            case DecisionOutcomeType.Correct:

                decisionPath += "C";
                CorrectChoices++;

                break;


            case DecisionOutcomeType.Incorrect:

                decisionPath += "L";
                IncorrectChoices++;

                break;


            case DecisionOutcomeType.Death:

                DeathChoices++;

                break;
        }


        DecisionRecord existingRecord =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );


        existingRecord.selectedOptionIndex =
            optionIndex;

        existingRecord.finalOutcome =
            outcome;


        if (diedAtLeastOnce)
        {
            existingRecord.diedAtLeastOnce =
                true;
        }
    }



    public void RegisterFinalDecision(
        string decisionID,
        string nodeID,
        int optionIndex,
        DecisionOutcomeType outcome,
        bool diedAtLeastOnce
    )
    {
        DecisionRecord existingRecord =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );


        existingRecord.nodeID =
            nodeID;

        existingRecord.selectedOptionIndex =
            optionIndex;

        existingRecord.finalOutcome =
            outcome;


        if (diedAtLeastOnce)
        {
            existingRecord.diedAtLeastOnce =
                true;
        }
    }



    public void RegisterDeath(
        string decisionID,
        string nodeID
    )
    {

        DeathChoices++;


        DecisionRecord existingRecord =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );


        existingRecord.nodeID =
            nodeID;

        existingRecord.diedAtLeastOnce =
            true;
    }



    private DecisionRecord GetOrCreateDecisionRecord(
        string decisionID,
        string nodeID
    )
    {
        foreach (
            DecisionRecord record
            in decisionRecords
        )
        {
            if (
                record != null &&
                record.decisionID == decisionID
            )
            {
                if (!string.IsNullOrEmpty(nodeID))
                {
                    record.nodeID =
                        nodeID;
                }

                return record;
            }
        }


        DecisionRecord newRecord =
            new DecisionRecord
            {
                decisionID =
                    decisionID,

                nodeID =
                    nodeID,

                decisionOrder =
                    decisionRecords.Count + 1,

                selectedOptionIndex =
                    -1,

                finalOutcome =
                    DecisionOutcomeType.Incorrect,

                diedAtLeastOnce =
                    false,

                retryCount =
                    0,

                totalTimeSeconds =
                    0f,

                attempts =
                    new List<DecisionAttemptRecord>()
            };


        decisionRecords.Add(
            newRecord
        );


        return newRecord;
    }



    public void SetCompanion(
        bool valentina,
        bool beto
    )
    {
        hasValentina = valentina;
        hasBeto = beto;
    }



    public void ResetData()
    {
        score = 0;

        hasValentina = false;
        hasBeto = false;

        currentNode = "";
        decisionPath = "";

        decisionHistory.Clear();
        decisionRecords.Clear();

        CorrectChoices = 0;
        IncorrectChoices = 0;
        DeathChoices = 0;



        activeDecisionID = "";

        activeDecisionStartTime = 0f;

        activeDecisionAttemptNumber = 0;
    }
}
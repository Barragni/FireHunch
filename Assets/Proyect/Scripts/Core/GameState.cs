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


    // ============================================================
    // CONTROL DEL INTENTO ACTUAL
    // ============================================================

    // ID de la decisión que actualmente está siendo respondida.
    private string activeDecisionID = "";

    // Momento en que comenzó el intento actual.
    private float activeDecisionStartTime;

    // Número del intento actual de esa decisión.
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


    // ============================================================
    // PUNTAJE
    // ============================================================

    public void AddScore(int amount)
    {
        score += amount;
    }


    // ============================================================
    // INICIAR INTENTO DE DECISIÓN
    // ============================================================

    /// <summary>
    /// Se llama cuando aparece el panel de una decisión.
    ///
    /// Aquí comienza a medirse el tiempo de ESTE intento.
    /// </summary>
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


    // ============================================================
    // FINALIZAR INTENTO DE DECISIÓN
    // ============================================================

    /// <summary>
    /// Registra un intento individual de una decisión.
    ///
    /// Ejemplo:
    ///
    /// Intento 1 → opción 0 → Death → 7.3 segundos
    /// Intento 2 → opción 1 → Correct → 11.4 segundos
    /// </summary>
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


        // ========================================================
        // OBTENER / CREAR REGISTRO
        // ========================================================

        DecisionRecord record =
            GetOrCreateDecisionRecord(
                decisionID,
                nodeID
            );


        // ========================================================
        // CALCULAR TIEMPO
        // ========================================================

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


        // ========================================================
        // NÚMERO DE INTENTO
        // ========================================================

        int attemptNumber =
            record.attempts.Count + 1;


        // ========================================================
        // CREAR REGISTRO DEL INTENTO
        // ========================================================

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


        // ========================================================
        // ACTUALIZAR ESTADÍSTICAS
        // ========================================================

        record.totalTimeSeconds +=
            elapsedTime;


        // Una muerte significa que este intento
        // terminó necesitando un reintento.
        if (outcome == DecisionOutcomeType.Death)
        {
            record.retryCount++;

            record.diedAtLeastOnce =
                true;
        }


        // ========================================================
        // FINALIZÓ EL INTENTO ACTUAL
        // ========================================================

        activeDecisionID = "";

        activeDecisionStartTime = 0f;

        activeDecisionAttemptNumber = 0;
    }


    // ============================================================
    // REGISTRO DE DECISIÓN
    // ============================================================

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


    // ============================================================
    // REGISTRO DE RUTA
    // ============================================================

    /// <summary>
    /// Registra únicamente decisiones definitivas.
    ///
    /// C = Correcta
    /// L = Incorrecta
    ///
    /// Muerte = no modifica decisionPath.
    /// </summary>
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

                // Una muerte no es una decisión definitiva.
                // Solamente cuenta como reintento.
                DeathChoices++;

                break;


            default:

                Debug.LogWarning(
                    $"GameState: Resultado de decisión desconocido '{result}'."
                );

                break;
        }
    }


    // ============================================================
    // REGISTRO DETALLADO
    // ============================================================

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

                // No agregar M a la ruta.
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


    // ============================================================
    // REGISTRO DE DECISIÓN FINAL
    // ============================================================

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


    // ============================================================
    // REGISTRO DE MUERTE
    // ============================================================

    public void RegisterDeath(
        string decisionID,
        string nodeID
    )
    {
        // ========================================================
        // IMPORTANTE:
        //
        // NO agregar "M" a decisionPath.
        //
        // El jugador todavía no ha tomado una decisión definitiva.
        // ========================================================

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


    // ============================================================
    // OBTENER / CREAR DECISION RECORD
    // ============================================================

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


    // ============================================================
    // COMPAÑEROS
    // ============================================================

    public void SetCompanion(
        bool valentina,
        bool beto
    )
    {
        hasValentina = valentina;
        hasBeto = beto;
    }


    // ============================================================
    // REINICIAR
    // ============================================================

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


        // Limpiar también el intento activo.

        activeDecisionID = "";

        activeDecisionStartTime = 0f;

        activeDecisionAttemptNumber = 0;
    }
}
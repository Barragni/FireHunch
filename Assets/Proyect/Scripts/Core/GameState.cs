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
    /// Registra únicamente la letra correspondiente
    /// a la ruta narrativa:
    ///
    /// C = Correcta
    /// L = Incorrecta
    /// M = Muerte
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

                decisionPath += "M";
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
        // ========================================================
        // HISTORIAL SIMPLE
        // ========================================================

        decisionHistory.Add(decisionID);

        // ========================================================
        // RUTA Y CONTADORES
        // ========================================================

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

                decisionPath += "M";
                DeathChoices++;

                break;
        }

        // ========================================================
        // BUSCAR REGISTRO EXISTENTE
        // ========================================================

        DecisionRecord existingRecord = null;

        foreach (DecisionRecord record in decisionRecords)
        {
            if (
                record != null &&
                record.decisionID == decisionID
            )
            {
                existingRecord = record;
                break;
            }
        }

        // ========================================================
        // ACTUALIZAR REGISTRO EXISTENTE
        // ========================================================

        if (existingRecord != null)
        {
            existingRecord.nodeID = nodeID;

            existingRecord.selectedOptionIndex =
                optionIndex;

            existingRecord.finalOutcome =
                outcome;

            if (diedAtLeastOnce)
            {
                existingRecord.diedAtLeastOnce = true;
            }

            return;
        }

        // ========================================================
        // CREAR NUEVO REGISTRO
        // ========================================================

        DecisionRecord newRecord =
            new DecisionRecord
            {
                decisionID = decisionID,
                nodeID = nodeID,
                decisionOrder =
                    decisionRecords.Count + 1,
                selectedOptionIndex =
                    optionIndex,
                finalOutcome =
                    outcome,
                diedAtLeastOnce =
                    diedAtLeastOnce
            };

        decisionRecords.Add(newRecord);
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
        // ========================================================
        // BUSCAR REGISTRO EXISTENTE
        // ========================================================

        DecisionRecord existingRecord = null;

        foreach (DecisionRecord record in decisionRecords)
        {
            if (
                record != null &&
                record.decisionID == decisionID
            )
            {
                existingRecord = record;
                break;
            }
        }

        // ========================================================
        // ACTUALIZAR REGISTRO EXISTENTE
        // ========================================================

        if (existingRecord != null)
        {
            existingRecord.nodeID = nodeID;

            existingRecord.selectedOptionIndex =
                optionIndex;

            existingRecord.finalOutcome =
                outcome;

            if (diedAtLeastOnce)
            {
                existingRecord.diedAtLeastOnce = true;
            }

            return;
        }

        // ========================================================
        // CREAR NUEVO REGISTRO
        // ========================================================

        DecisionRecord newRecord =
            new DecisionRecord
            {
                decisionID = decisionID,
                nodeID = nodeID,
                decisionOrder =
                    decisionRecords.Count + 1,
                selectedOptionIndex =
                    optionIndex,
                finalOutcome =
                    outcome,
                diedAtLeastOnce =
                    diedAtLeastOnce
            };

        decisionRecords.Add(newRecord);
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
        // RUTA
        // ========================================================

        decisionPath += "M";
        DeathChoices++;

        // ========================================================
        // BUSCAR REGISTRO EXISTENTE
        // ========================================================

        DecisionRecord existingRecord = null;

        foreach (DecisionRecord record in decisionRecords)
        {
            if (
                record != null &&
                record.decisionID == decisionID
            )
            {
                existingRecord = record;
                break;
            }
        }

        // ========================================================
        // SI YA EXISTE, MARCAR REINTENTO
        // ========================================================

        if (existingRecord != null)
        {
            existingRecord.nodeID = nodeID;
            existingRecord.diedAtLeastOnce = true;

            return;
        }

        // ========================================================
        // CREAR REGISTRO
        // ========================================================

        DecisionRecord newRecord =
            new DecisionRecord
            {
                decisionID = decisionID,
                nodeID = nodeID,
                decisionOrder =
                    decisionRecords.Count + 1,
                selectedOptionIndex = -1,
                finalOutcome =
                    DecisionOutcomeType.Death,
                diedAtLeastOnce = true
            };

        decisionRecords.Add(newRecord);
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
    }
}
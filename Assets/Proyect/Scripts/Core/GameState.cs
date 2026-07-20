using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // ==========================
    // Estadísticas de la partida
    // ==========================

    [Header("Registro de decisiones")]
    public List<DecisionRecord> decisionRecords = new();

    // Evita contar varias veces la muerte
    // de una misma decisión.
    private readonly HashSet<string> countedDeaths = new();

    // Lleva el orden en que el jugador
    // resolvió las decisiones.
    private int nextDecisionOrder = 1;

    // ==========================
    // Narrativa
    // ==========================

    [Header("Compañeros actuales")]
    public bool hasValentina;
    public bool hasBeto;

    [Header("Nodo narrativo actual")]
    public string currentNode;

    [Header("Ruta narrativa (C, L)")]
    public string decisionPath = "";

    [Header("Historial completo (compatibilidad)")]
    public List<string> decisionHistory = new();

    [Header("Banderas narrativas")]
    public List<string> narrativeFlags = new();

    public int CorrectChoices
    {
        get
        {
            int count = 0;

            foreach (DecisionRecord record in decisionRecords)
            {
                if (record.finalOutcome == DecisionOutcomeType.Correct)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int IncorrectChoices
    {
        get
        {
            int count = 0;

            foreach (DecisionRecord record in decisionRecords)
            {
                if (record.finalOutcome == DecisionOutcomeType.Incorrect)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int DeathChoices
    {
        get
        {
            int count = 0;

            foreach (DecisionRecord record in decisionRecords)
            {
                if (record.diedAtLeastOnce)
                {
                    count++;
                }
            }

            return count;
        }
    }

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

    // ====================================================
    // REGISTRO DE DECISIONES
    // ====================================================

    public DecisionRecord GetDecisionRecord(string decisionID)
    {
        foreach (DecisionRecord record in decisionRecords)
        {
            if (record.decisionID == decisionID)
            {
                return record;
            }
        }

        return null;
    }

    public DecisionRecord CreateDecisionRecord(string decisionID)
    {
        DecisionRecord record = GetDecisionRecord(decisionID);

        if (record != null)
        {
            return record;
        }

        record = new DecisionRecord
        {
            decisionID = decisionID,
            decisionOrder = nextDecisionOrder++
        };

        decisionRecords.Add(record);

        return record;
    }

    public void RegisterDeath(string decisionID)
    {
        if (string.IsNullOrWhiteSpace(decisionID))
        {
            Debug.LogWarning("DecisionID vacío. No se registró la muerte.");
            return;
        }

        if (countedDeaths.Add(decisionID))
        {
            DecisionRecord record = CreateDecisionRecord(decisionID);
            record.diedAtLeastOnce = true;
        }
    }

    public void RegisterFinalDecision(
        string decisionID,
        DecisionOutcomeType outcome,
        int selectedOptionIndex)
    {
        DecisionRecord record = CreateDecisionRecord(decisionID);

        record.finalOutcome = outcome;
        record.selectedOptionIndex = selectedOptionIndex;
    }

    // ====================================================
    // Narrativa
    // ====================================================

    public void RegisterDecision(string decision)
    {
        decisionHistory.Add(decision);

        decisionPath += decision;
    }

    public void SetCompanion(
        string companionId,
        bool value)
    {
        switch (companionId)
        {
            case "Valentina":
                hasValentina = value;
                break;

            case "Beto":
                hasBeto = value;
                break;
        }
    }

    public void AddFlag(string flag)
    {
        if (!narrativeFlags.Contains(flag))
        {
            narrativeFlags.Add(flag);
        }
    }

    public void RemoveFlag(string flag)
    {
        narrativeFlags.Remove(flag);
    }

    public bool HasFlag(string flag)
    {
        return narrativeFlags.Contains(flag);
    }

    public void ResetData()
    {
        decisionRecords.Clear();

        countedDeaths.Clear();

        nextDecisionOrder = 1;

        hasValentina = false;
        hasBeto = false;

        currentNode = string.Empty;

        decisionPath = "";

        decisionHistory.Clear();

        narrativeFlags.Clear();
    }
}
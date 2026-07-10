using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [Header("Puntaje total obtenido")]
    public int score;

    [Header("Compañeros actuales")]
    public bool hasValentina;
    public bool hasBeto;

    [Header("Nodo narrativo actual")]
    public string currentNode;

    [Header("Historial de decisiones (C, L, M)")]
    public string decisionPath = "";

    [Header("Historial completo (compatibilidad)")]
    public List<string> decisionHistory = new();

    [Header("Banderas narrativas")]
    public List<string> narrativeFlags = new();

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

    public void AddScore(int points)
    {
        score += points;
    }

    public void RegisterDecision(string decision)
    {
        decisionHistory.Add(decision);
        decisionPath += decision;
    }

    public void SetCompanion(string companionId, bool value)
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
        score = 0;

        hasValentina = false;
        hasBeto = false;

        currentNode = string.Empty;

        decisionPath = "";

        decisionHistory.Clear();

        narrativeFlags.Clear();
    }
}
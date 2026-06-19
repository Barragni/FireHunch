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

    [Header("Historial de decisiones")]
    public List<string> decisionHistory = new();

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

    public void RegisterDecision(string decisionId)
    {
        decisionHistory.Add(decisionId);
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

    public void ResetData()
    {
        score = 0;

        hasValentina = false;
        hasBeto = false;

        currentNode = string.Empty;

        decisionHistory.Clear();
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class AttemptData
{
    // ==========================
    // Identificación
    // ==========================

    public string attemptId;

    public string timestampUtc;

    // ==========================
    // Participante
    // ==========================

    public int age;

    public bool consentAccepted;

    // ==========================
    // Información de la sesión
    // ==========================

    public float durationSeconds;

    public string platform;

    public string unityVersion;

    public string gameVersion;

    // ==========================
    // Resultado final
    // ==========================

    public float score;

    public string ending;

    public string decisionPath;

    public int correctChoices;

    public int incorrectChoices;

    public int deathChoices;

    // ==========================
    // Decisiones
    // ==========================

    public List<AttemptDecisionData> decisions = new();
}

[Serializable]
public class AttemptDecisionData
{
    public string decisionID;

    public int decisionOrder;

    public int selectedOptionIndex;

    public string finalOutcome;

    public bool diedAtLeastOnce;

    public float timeSeconds;
}
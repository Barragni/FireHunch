using System;
using System.Collections.Generic;

[Serializable]
public class AttemptData
{

    public string attemptId;

    public string timestampUtc;



    public int age;

    public bool consentAccepted;


    public float durationSeconds;

    public string platform;



    public float score;

    public string ending;

    public string decisionPath;


    public int correctChoices;

    public int incorrectChoices;

    public int totalRetries;


    public int deathChoices;



    public List<AttemptDecisionData> decisions = new();
}



[Serializable]
public class AttemptDecisionData
{
    public string decisionID;

    public int decisionOrder;


    public int finalSelectedOptionIndex;

    public string finalOutcome;


    public int retryCount;


    public float totalTimeSeconds;


    public List<DecisionAttemptRecord> attempts = new();
}
using System;
using System.Collections.Generic;

[Serializable]
public class DecisionRecord
{
    public string decisionID;

    public string nodeID;

    public int decisionOrder;


    public int selectedOptionIndex = -1;

    public DecisionOutcomeType finalOutcome;

    public bool diedAtLeastOnce;


    public int retryCount;


    public float totalTimeSeconds;


    public List<DecisionAttemptRecord> attempts = new();
}



[Serializable]
public class DecisionAttemptRecord
{
    public int attemptNumber;

    public int selectedOptionIndex = -1;

    public DecisionOutcomeType outcome;

    public float timeSeconds;
}
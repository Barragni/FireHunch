using UnityEngine;

public static class AttemptMapper
{
    public static AttemptData CreateAttempt(
        float finalScore,
        string ending)
    {
        if (GameState.Instance == null)
        {
            Debug.LogError("GameState.Instance es null.");
            return null;
        }

        if (AttemptSession.Instance == null)
        {
            Debug.LogError("AttemptSession.Instance es null.");
            return null;
        }

        AttemptSession.Instance.FinishSession();

        int correctChoices = 0;
        int incorrectChoices = 0;
        int retryChoices = 0;

        AttemptData attempt = new AttemptData
        {
            // ==========================
            // Identificación
            // ==========================

            attemptId = AttemptSession.Instance.AttemptId,

            timestampUtc =
                AttemptSession.Instance.StartTimeUtc.ToString("o"),

            // ==========================
            // Participante
            // ==========================

            age =
                AttemptSession.Instance.Age,

            consentAccepted =
                AttemptSession.Instance.ConsentAccepted,

            // ==========================
            // Información de la sesión
            // ==========================

            durationSeconds =
                AttemptSession.Instance.DurationSeconds,

            platform =
                Application.platform.ToString(),

            // ==========================
            // Resultado final
            // ==========================

            score = finalScore,

            ending = ending,

            decisionPath =
                GameState.Instance.decisionPath
        };

        foreach (DecisionRecord record in GameState.Instance.decisionRecords)
        {
            switch (record.finalOutcome)
            {
                case DecisionOutcomeType.Correct:
                    correctChoices++;
                    break;

                case DecisionOutcomeType.Incorrect:
                    incorrectChoices++;
                    break;
            }

            if (record.diedAtLeastOnce)
            {
                retryChoices++;
            }

            AttemptDecisionData decision =
                new AttemptDecisionData
                {
                    decisionID = record.decisionID,

                    decisionOrder = record.decisionOrder,

                    selectedOptionIndex =
                        record.selectedOptionIndex,

                    finalOutcome =
                        record.finalOutcome.ToString(),

                    diedAtLeastOnce =
                        record.diedAtLeastOnce,

                    // TODO: Se llenará cuando implementemos
                    // el tiempo por decisión.
                    timeSeconds = 0f
                };

            attempt.decisions.Add(decision);
        }

        attempt.correctChoices = correctChoices;
        attempt.incorrectChoices = incorrectChoices;
        attempt.deathChoices = retryChoices;

        return attempt;
    }
}
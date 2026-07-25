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

            unityVersion =
                Application.unityVersion,

            gameVersion =
                Application.version,

            // ==========================
            // Resultado final
            // ==========================

            score = finalScore,

            ending = ending,

            decisionPath =
                GameState.Instance.decisionPath,

            correctChoices =
                GameState.Instance.CorrectChoices,

            incorrectChoices =
                GameState.Instance.IncorrectChoices,

            deathChoices =
                GameState.Instance.DeathChoices
        };

        foreach (DecisionRecord record in GameState.Instance.decisionRecords)
        {
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

                    // TODO: Este valor se llenará cuando
                    // implementemos el tiempo por decisión.
                    timeSeconds = 0f
                };

            attempt.decisions.Add(decision);
        }

        return attempt;
    }
}
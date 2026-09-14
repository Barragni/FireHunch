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
        int totalRetries = 0;

        AttemptData attempt = new AttemptData
        {
            // ==========================
            // Identificación
            // ==========================

            attemptId =
                AttemptSession.Instance.AttemptId,

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

            score =
                finalScore,

            ending =
                ending,

            decisionPath =
                GameState.Instance.decisionPath
        };


        // ============================================================
        // DECISIONES
        // ============================================================

        foreach (
            DecisionRecord record
            in GameState.Instance.decisionRecords
        )
        {
            if (record == null)
            {
                continue;
            }


            // ========================================================
            // CONTADORES GENERALES
            // ========================================================

            switch (record.finalOutcome)
            {
                case DecisionOutcomeType.Correct:

                    correctChoices++;

                    break;

                case DecisionOutcomeType.Incorrect:

                    incorrectChoices++;

                    break;
            }


            // ========================================================
            // REINTENTOS
            // ========================================================

            totalRetries +=
                record.retryCount;


            // ========================================================
            // CREAR DATOS DE LA DECISIÓN
            // ========================================================

            AttemptDecisionData decision =
                new AttemptDecisionData
                {
                    decisionID =
                        record.decisionID,

                    decisionOrder =
                        record.decisionOrder,

                    finalSelectedOptionIndex =
                        record.selectedOptionIndex,

                    finalOutcome =
                        record.finalOutcome.ToString(),

                    retryCount =
                        record.retryCount,

                    totalTimeSeconds =
                        record.totalTimeSeconds
                };


            // ========================================================
            // COPIAR HISTORIAL DE INTENTOS
            // ========================================================

            if (
                record.attempts != null &&
                record.attempts.Count > 0
            )
            {
                foreach (
                    DecisionAttemptRecord recordAttempt
                    in record.attempts
                )
                {
                    if (recordAttempt == null)
                    {
                        continue;
                    }

                    decision.attempts.Add(
                        new DecisionAttemptRecord
                        {
                            attemptNumber =
                                recordAttempt.attemptNumber,

                            selectedOptionIndex =
                                recordAttempt.selectedOptionIndex,

                            outcome =
                                recordAttempt.outcome,

                            timeSeconds =
                                recordAttempt.timeSeconds
                        }
                    );
                }
            }


            attempt.decisions.Add(
                decision
            );
        }


        // ============================================================
        // ESTADÍSTICAS GENERALES
        // ============================================================

        attempt.correctChoices =
            correctChoices;

        attempt.incorrectChoices =
            incorrectChoices;

        attempt.totalRetries =
            totalRetries;

        // Compatibilidad temporal con Google Sheets.
        attempt.deathChoices =
            GameState.Instance.DeathChoices;


        return attempt;
    }
}
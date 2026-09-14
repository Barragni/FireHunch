using System;
using System.Collections.Generic;

[Serializable]
public class DecisionRecord
{
    // ============================================================
    // IDENTIFICACIÓN
    // ============================================================

    // ID único de la DecisionSequence.
    public string decisionID;

    // ID del NarrativeNode / escenario.
    public string nodeID;

    // Orden en que el jugador resolvió esta decisión.
    public int decisionOrder;


    // ============================================================
    // RESULTADO FINAL
    // ============================================================

    // Índice de la opción finalmente elegida.
    public int selectedOptionIndex = -1;

    // Resultado final de la decisión.
    // Normalmente será Correct o Incorrect.
    public DecisionOutcomeType finalOutcome;

    // Si el jugador murió al menos una vez
    // antes de resolver definitivamente la decisión.
    public bool diedAtLeastOnce;


    // ============================================================
    // REINTENTOS
    // ============================================================

    // Cantidad de veces que el jugador tuvo que
    // volver a intentar esta decisión.
    public int retryCount;


    // ============================================================
    // TIEMPO
    // ============================================================

    // Tiempo total empleado en todos los intentos
    // de esta decisión.
    public float totalTimeSeconds;


    // ============================================================
    // HISTORIAL DE INTENTOS
    // ============================================================

    // Cada elemento representa una vez que el jugador
    // enfrentó esta decisión.
    public List<DecisionAttemptRecord> attempts = new();
}


// ================================================================
// REGISTRO DE UN INTENTO INDIVIDUAL
// ================================================================

[Serializable]
public class DecisionAttemptRecord
{
    // Número de intento dentro de esta decisión.
    // 1 = primer intento.
    // 2 = primer reintento.
    // 3 = segundo reintento, etc.
    public int attemptNumber;

    // Opción que seleccionó el jugador.
    public int selectedOptionIndex = -1;

    // Resultado de ESTE intento.
    //
    // Correct
    // Incorrect
    // Death
    public DecisionOutcomeType outcome;

    // Tiempo que tardó el jugador en resolver
    // este intento concreto.
    public float timeSeconds;
}
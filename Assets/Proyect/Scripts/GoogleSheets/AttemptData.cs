using System;
using System.Collections.Generic;

[Serializable]
public class AttemptData
{
    // ============================================================
    // IDENTIFICACIÓN
    // ============================================================

    public string attemptId;

    public string timestampUtc;


    // ============================================================
    // PARTICIPANTE
    // ============================================================

    public int age;

    public bool consentAccepted;


    // ============================================================
    // INFORMACIÓN DE LA SESIÓN
    // ============================================================

    // Tiempo total desde que comenzó la sesión
    // hasta que terminó el juego.
    public float durationSeconds;

    public string platform;


    // ============================================================
    // RESULTADO FINAL
    // ============================================================

    public float score;

    public string ending;

    // Ejemplo: "CLC"
    public string decisionPath;


    // ============================================================
    // ESTADÍSTICAS GENERALES
    // ============================================================

    // Cantidad de decisiones definitivas correctas.
    public int correctChoices;

    // Cantidad de decisiones definitivas incorrectas.
    public int incorrectChoices;

    // Cantidad TOTAL de reintentos durante toda la sesión.
    //
    // Ejemplo:
    // Decisión 1 → 1 reintento
    // Decisión 2 → 2 reintentos
    // Decisión 3 → 0 reintentos
    //
    // totalRetries = 3
    public int totalRetries;


    // ============================================================
    // COMPATIBILIDAD CON EL SISTEMA ACTUAL
    // ============================================================

    // Este campo lo mantenemos temporalmente para no romper
    // el sistema actual de Google Sheets mientras migramos.
    //
    // Más adelante podremos eliminarlo si ya no lo necesitamos.
    public int deathChoices;


    // ============================================================
    // DECISIONES
    // ============================================================

    public List<AttemptDecisionData> decisions = new();
}


// =================================================================
// DATOS DE UNA DECISIÓN
// =================================================================

[Serializable]
public class AttemptDecisionData
{
    // ID de la decisión.
    public string decisionID;

    // Orden en que apareció durante la sesión.
    public int decisionOrder;


    // ============================================================
    // RESULTADO FINAL DE LA DECISIÓN
    // ============================================================

    // Opción que finalmente permitió resolver la decisión.
    public int finalSelectedOptionIndex;

    // Resultado definitivo:
    // Correct / Incorrect
    public string finalOutcome;


    // ============================================================
    // REINTENTOS
    // ============================================================

    // Cantidad de veces que el jugador tuvo que reintentar
    // esta decisión.
    //
    // 0 = la resolvió a la primera.
    // 1 = murió una vez y volvió a intentarlo.
    // 2 = murió dos veces, etc.
    public int retryCount;


    // ============================================================
    // TIEMPO
    // ============================================================

    // Tiempo total empleado en esta decisión,
    // sumando todos sus intentos.
    public float totalTimeSeconds;


    // ============================================================
    // HISTORIAL DE INTENTOS
    // ============================================================

    // Contiene cada intento individual de esta decisión.
    public List<DecisionAttemptRecord> attempts = new();
}
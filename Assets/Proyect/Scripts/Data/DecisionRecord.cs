using System;

[Serializable]
public class DecisionRecord
{
    // ID único de la DecisionSequence.
    public string decisionID;

    // Orden en que el jugador resolvió esta decisión.
    public int decisionOrder;

    // Índice de la opción finalmente elegida.
    public int selectedOptionIndex = -1;

    // Resultado final (Correct o Incorrect).
    public DecisionOutcomeType finalOutcome;

    // Si murió al menos una vez antes de resolverla.
    public bool diedAtLeastOnce;
}
using TMPro;
using UnityEngine;

public class ResultsManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Penalización por muerte")]
    [Range(0f, 1f)]
    [SerializeField] private float deathPenalty = 0.25f;

    private void Start()
    {
        ShowResults();
    }

    private void ShowResults()
    {
        float score = CalculateScore();

        scoreText.text = $"{score:F1} / 10";

        Evaluation evaluation = GetEvaluation(score);

        endingText.text = evaluation.title;
        feedbackText.text = evaluation.feedback;
    }

    private float CalculateScore()
    {
        int correct = GameState.Instance.CorrectChoices;
        int incorrect = GameState.Instance.IncorrectChoices;
        int deaths = GameState.Instance.DeathChoices;

        int totalDecisions = correct + incorrect;

        if (totalDecisions == 0)
            return 1f;

        float accuracy = (float)correct / totalDecisions;

        float deathFactor = (float)deaths / totalDecisions;

        float finalScore = (accuracy - deathFactor * deathPenalty) * 10f;

        return Mathf.Clamp(finalScore, 1f, 10f);
    }

    private Evaluation GetEvaluation(float score)
    {
        if (score >= 9.5f)
        {
            return new Evaluation(
                "Experto en Supervivencia",
                "Demostraste un criterio sobresaliente para priorizar tu seguridad y actuar correctamente durante la emergencia."
            );
        }

        if (score >= 8f)
        {
            return new Evaluation(
                "Muy Buen Criterio",
                "Tomaste la mayoría de las decisiones adecuadas y resolviste la emergencia de forma segura."
            );
        }

        if (score >= 6.5f)
        {
            return new Evaluation(
                "Buen Criterio",
                "Aunque acertaste en varias decisiones, todavía existen aspectos importantes que podrías mejorar."
            );
        }

        if (score >= 5f)
        {
            return new Evaluation(
                "Criterio Insuficiente",
                "Varias decisiones aumentaron innecesariamente el riesgo durante la evacuación."
            );
        }

        return new Evaluation(
            "Alto Riesgo",
            "Tus decisiones comprometieron gravemente tu supervivencia. Se recomienda reforzar los conocimientos sobre evacuación en incendios."
        );
    }

    private struct Evaluation
    {
        public string title;
        public string feedback;

        public Evaluation(string title, string feedback)
        {
            this.title = title;
            this.feedback = feedback;
        }
    }
}
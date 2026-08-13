using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionReviewCard : MonoBehaviour
{
    [Header("Encabezado")]
    [SerializeField] private TMP_Text decisionNumberText;
    [SerializeField] private TMP_Text resultText;

    [Header("Imagen")]
    [SerializeField] private Image decisionImage;

    [Header("Contenido")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text selectedOptionText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Indicador de reintento")]
    [SerializeField] private TMP_Text retryText;

    public void Setup(
        DecisionRecord record,
        DecisionSequence decision
    )
    {
        if (record == null)
        {
            Debug.LogError(
                "DecisionReviewCard: DecisionRecord es null."
            );

            return;
        }

        if (decision == null)
        {
            Debug.LogError(
                $"DecisionReviewCard: DecisionSequence es null para '{record.decisionID}'."
            );

            return;
        }

        // ========================================================
        // NÚMERO DE DECISIÓN
        // ========================================================

        if (decisionNumberText != null)
        {
            decisionNumberText.text =
                $"DECISIÓN {record.decisionOrder}";
        }

        // ========================================================
        // PREGUNTA
        // ========================================================

        if (questionText != null)
        {
            questionText.text =
                decision.question;
        }

        // ========================================================
        // OPCIÓN FINALMENTE SELECCIONADA
        // ========================================================

        DecisionOption selectedOption = null;

        if (
            decision.options != null &&
            record.selectedOptionIndex >= 0 &&
            record.selectedOptionIndex <
            decision.options.Count
        )
        {
            selectedOption =
                decision.options[
                    record.selectedOptionIndex
                ];
        }

        if (selectedOption != null)
        {
            // ----------------------------------------------------
            // Texto de la elección
            // ----------------------------------------------------

            if (selectedOptionText != null)
            {
                selectedOptionText.text =
                    selectedOption.optionText;
            }

            // ----------------------------------------------------
            // Imagen
            // ----------------------------------------------------

            if (decisionImage != null)
            {
                decisionImage.sprite =
                    selectedOption.reviewImage;

                decisionImage.enabled =
                    selectedOption.reviewImage != null;
            }

            // ----------------------------------------------------
            // Retroalimentación
            // ----------------------------------------------------

            if (feedbackText != null)
            {
                feedbackText.text =
                    selectedOption.reviewFeedback;
            }
        }
        else
        {
            // No existe una elección válida registrada.

            if (selectedOptionText != null)
            {
                selectedOptionText.text =
                    "No se registró una elección final.";
            }

            if (decisionImage != null)
            {
                decisionImage.sprite = null;
                decisionImage.enabled = false;
            }

            if (feedbackText != null)
            {
                feedbackText.text =
                    "Esta decisión terminó antes de registrarse una elección final.";
            }
        }

        // ========================================================
        // RESULTADO
        // ========================================================

        if (resultText != null)
        {
            switch (record.finalOutcome)
            {
                case DecisionOutcomeType.Correct:

                    resultText.text =
                        "✓ CORRECTA";

                    break;

                case DecisionOutcomeType.Incorrect:

                    resultText.text =
                        "✕ INCORRECTA";

                    break;

                case DecisionOutcomeType.Death:

                    resultText.text =
                        "✕ MUERTE";

                    break;

                default:

                    resultText.text =
                        "RESULTADO";

                    break;
            }
        }

        // ========================================================
        // INDICADOR DE REINTENTO
        // ========================================================

        if (retryText != null)
        {
            retryText.gameObject.SetActive(
                record.diedAtLeastOnce
            );
        }
    }
}
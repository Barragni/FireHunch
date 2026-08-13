using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionReviewUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject reviewPanel;

    [Header("Contenido")]
    [SerializeField] private Transform content;

    [Header("Prefab de tarjeta")]
    [SerializeField] private DecisionReviewCard cardPrefab;

    [Header("Botón cerrar")]
    [SerializeField] private Button closeButton;

    private readonly List<DecisionReviewCard> cards = new();

    private void Awake()
    {
        if (
            reviewPanel != null &&
            reviewPanel != gameObject
        )
        {
            reviewPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseReview);
            closeButton.onClick.AddListener(CloseReview);
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseReview);
        }
    }

    public void OpenReview()
    {
        Debug.Log("========================================");
        Debug.Log("DecisionReviewUI: ABRIENDO REVISIÓN");
        Debug.Log("========================================");

        if (reviewPanel == null)
        {
            Debug.LogError(
                "DecisionReviewUI: REVIEW PANEL NO ASIGNADO."
            );

            return;
        }

        reviewPanel.SetActive(true);

        BuildReview();
    }

    public void CloseReview()
    {
        if (reviewPanel != null)
        {
            reviewPanel.SetActive(false);
        }
    }

    private void BuildReview()
    {
        Debug.Log("DecisionReviewUI: Construyendo tarjetas...");

        ClearCards();

        // ========================================================
        // GAME STATE
        // ========================================================

        if (GameState.Instance == null)
        {
            Debug.LogError(
                "DecisionReviewUI: GameState.Instance es NULL."
            );

            return;
        }

        Debug.Log(
            $"DecisionReviewUI: Hay {GameState.Instance.decisionRecords.Count} DecisionRecord(s)."
        );

        // ========================================================
        // NARRATIVE MANAGER
        // ========================================================

        if (NarrativeManager.Instance == null)
        {
            Debug.LogError(
                "DecisionReviewUI: NarrativeManager.Instance es NULL."
            );

            return;
        }

        // ========================================================
        // CONTENT
        // ========================================================

        if (content == null)
        {
            Debug.LogError(
                "DecisionReviewUI: CONTENT NO ESTÁ ASIGNADO."
            );

            return;
        }

        Debug.Log(
            $"DecisionReviewUI: Content asignado: {content.name}"
        );

        // ========================================================
        // PREFAB
        // ========================================================

        if (cardPrefab == null)
        {
            Debug.LogError(
                "DecisionReviewUI: CARD PREFAB NO ESTÁ ASIGNADO."
            );

            return;
        }

        Debug.Log(
            $"DecisionReviewUI: Card Prefab asignado: {cardPrefab.name}"
        );

        // ========================================================
        // ORDENAR DECISIONES
        // ========================================================

        List<DecisionRecord> records =
            new List<DecisionRecord>(
                GameState.Instance.decisionRecords
            );

        records.Sort(
            (a, b) =>
                a.decisionOrder.CompareTo(
                    b.decisionOrder
                )
        );

        // ========================================================
        // CREAR TARJETAS
        // ========================================================

        foreach (DecisionRecord record in records)
        {
            if (record == null)
            {
                Debug.LogWarning(
                    "DecisionReviewUI: Se encontró un DecisionRecord NULL."
                );

                continue;
            }

            Debug.Log(
                $"Procesando decisión: ID='{record.decisionID}' | Orden={record.decisionOrder} | Opción={record.selectedOptionIndex} | Resultado={record.finalOutcome}"
            );

            DecisionSequence decision =
                NarrativeManager.Instance.GetDecisionSequenceByID(
                    record.decisionID
                );

            if (decision == null)
            {
                Debug.LogError(
                    $"DecisionReviewUI: NO SE ENCONTRÓ DecisionSequence para ID '{record.decisionID}'."
                );

                continue;
            }

            Debug.Log(
                $"DecisionReviewUI: DecisionSequence encontrada: '{decision.name}'"
            );

            DecisionReviewCard card =
                Instantiate(
                    cardPrefab,
                    content
                );

            if (card == null)
            {
                Debug.LogError(
                    "DecisionReviewUI: Instantiate devolvió NULL."
                );

                continue;
            }

            Debug.Log(
                $"DecisionReviewUI: Tarjeta creada correctamente para '{record.decisionID}'."
            );

            card.Setup(
                record,
                decision
            );

            cards.Add(card);
        }

        // ========================================================
        // ACTUALIZAR LAYOUT
        // ========================================================

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            content as RectTransform
        );

        ScrollRect scrollRect =
            reviewPanel.GetComponentInChildren<ScrollRect>();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        Debug.Log(
            $"DecisionReviewUI: FIN. Tarjetas creadas: {cards.Count}"
        );

        Debug.Log("========================================");
    }

    private void ClearCards()
    {
        foreach (DecisionReviewCard card in cards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        cards.Clear();

        if (content == null)
            return;

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(
                content.GetChild(i).gameObject
            );
        }
    }
}
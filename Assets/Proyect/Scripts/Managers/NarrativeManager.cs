using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [SerializeField]
    private NarrativeDatabase database;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public NarrativeNode GetNode(string nodeID)
    {
        if (database == null)
        {
            Debug.LogError(
                "NarrativeManager: No existe un NarrativeDatabase asignado."
            );

            return null;
        }

        foreach (NarrativeNode node in database.nodes)
        {
            if (node == null)
                continue;

            if (node.nodeID == nodeID)
            {
                return node;
            }
        }

        Debug.LogError(
            $"No existe el nodo '{nodeID}'."
        );

        return null;
    }

    public NarrativeRule GetCurrentRule(string nodeID)
    {
        NarrativeNode node =
            GetNode(nodeID);

        if (node == null)
        {
            return null;
        }

        if (GameState.Instance == null)
        {
            Debug.LogError(
                "NarrativeManager: GameState.Instance es null."
            );

            return null;
        }

        NarrativeRule rule =
            node.GetRule(
                GameState.Instance.decisionPath
            );

        if (rule == null)
        {
            Debug.LogError(
                $"El nodo '{nodeID}' no tiene una regla para la ruta '{GameState.Instance.decisionPath}'."
            );
        }

        return rule;
    }

    public string GetSceneForNode(string nodeID)
    {
        NarrativeRule rule =
            GetCurrentRule(nodeID);

        if (rule == null)
        {
            return string.Empty;
        }

        return rule.sceneName;
    }

    public void SetCurrentNode(string nodeID)
    {
        if (GameState.Instance == null)
        {
            Debug.LogError(
                "NarrativeManager: GameState.Instance es null."
            );

            return;
        }

        GameState.Instance.currentNode = nodeID;
    }

    // ============================================================
    // REVISIÓN DE DECISIONES
    // ============================================================

    /// <summary>
    /// Busca una DecisionSequence dentro de todas las reglas
    /// del NarrativeDatabase utilizando su decisionID.
    /// </summary>
    public DecisionSequence GetDecisionSequenceByID(
        string decisionID
    )
    {
        if (string.IsNullOrWhiteSpace(decisionID))
        {
            Debug.LogWarning(
                "GetDecisionSequenceByID recibió un decisionID vacío."
            );

            return null;
        }

        if (database == null)
        {
            Debug.LogError(
                "NarrativeManager: No existe un NarrativeDatabase asignado."
            );

            return null;
        }

        foreach (NarrativeNode node in database.nodes)
        {
            if (node == null)
                continue;

            if (node.rules == null)
                continue;

            foreach (NarrativeRule rule in node.rules)
            {
                if (rule == null)
                    continue;

                if (rule.decision == null)
                    continue;

                if (
                    rule.decision.decisionID ==
                    decisionID
                )
                {
                    return rule.decision;
                }
            }
        }

        Debug.LogWarning(
            $"No se encontró una DecisionSequence con ID '{decisionID}'."
        );

        return null;
    }

    /// <summary>
    /// Busca el NarrativeNode que contiene la DecisionSequence
    /// correspondiente al decisionID indicado.
    ///
    /// Esto permite que la revisión obtenga la imagen y la
    /// retroalimentación propias del escenario.
    /// </summary>
    public NarrativeNode GetNodeByDecisionID(
        string decisionID
    )
    {
        if (string.IsNullOrWhiteSpace(decisionID))
        {
            Debug.LogWarning(
                "GetNodeByDecisionID recibió un decisionID vacío."
            );

            return null;
        }

        if (database == null)
        {
            Debug.LogError(
                "NarrativeManager: No existe un NarrativeDatabase asignado."
            );

            return null;
        }

        foreach (NarrativeNode node in database.nodes)
        {
            if (node == null)
                continue;

            if (node.rules == null)
                continue;

            foreach (NarrativeRule rule in node.rules)
            {
                if (rule == null)
                    continue;

                if (rule.decision == null)
                    continue;

                if (
                    rule.decision.decisionID ==
                    decisionID
                )
                {
                    return node;
                }
            }
        }

        Debug.LogWarning(
            $"No se encontró un NarrativeNode para la decisión '{decisionID}'."
        );

        return null;
    }
}
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
        foreach (NarrativeNode node in database.nodes)
        {
            if (node.nodeID == nodeID)
            {
                return node;
            }
        }

        Debug.LogError($"No existe el nodo '{nodeID}'.");

        return null;
    }

    public NarrativeRule GetCurrentRule(string nodeID)
    {
        NarrativeNode node = GetNode(nodeID);

        if (node == null)
        {
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
        GameState.Instance.currentNode = nodeID;
    }
}
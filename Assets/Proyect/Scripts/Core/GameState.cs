using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public int score;

    public bool hasValentina;
    public bool hasBeto;

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
}
using System;
using UnityEngine;

public class AttemptSession : MonoBehaviour
{
    public static AttemptSession Instance;

    public string AttemptId { get; private set; }

    public int Age { get; private set; }

    public bool ConsentAccepted { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public DateTime EndTimeUtc { get; private set; }

    public float DurationSeconds =>
        (float)(EndTimeUtc - StartTimeUtc).TotalSeconds;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StartSession(
        int age,
        bool consentAccepted)
    {
        AttemptId = Guid.NewGuid().ToString();

        Age = age;

        ConsentAccepted = consentAccepted;

        StartTimeUtc = DateTime.UtcNow;
    }

    public void FinishSession()
    {
        EndTimeUtc = DateTime.UtcNow;
    }
}
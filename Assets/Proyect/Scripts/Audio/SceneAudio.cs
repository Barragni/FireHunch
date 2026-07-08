using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [Header("Música")]
    [SerializeField] private AudioClip musicClip;

    [Header("Ambiente")]
    [SerializeField] private AudioClip ambienceClip;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "No existe AudioManager en la escena Bootstrap."
            );

            return;
        }

        AudioManager.Instance.PlayMusic(
            musicClip
        );

        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }
}
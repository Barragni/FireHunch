using UnityEngine;

public class SceneAudio : MonoBehaviour
{

    [Header("Música")]
    [SerializeField]
    private AudioClip musicClip;

    [Tooltip(
        "Si está activado, esta escena conserva exactamente la música " +
        "que venía reproduciéndose desde la escena anterior. " +
        "Si no había ninguna música sonando, utilizará Music Clip."
    )]
    [SerializeField]
    private bool keepPreviousMusic = false;


    [Header("Ambiente")]
    [SerializeField]
    private AudioClip ambienceClip;


    [Header("Volumen de esta escena")]

    [Tooltip(
        "Nivel de música propio de esta escena. " +
        "Se combina con el volumen configurado por el jugador."
    )]
    [Range(0f, 1f)]
    [SerializeField]
    private float musicVolume = 0.55f;

    [Tooltip(
        "Nivel del sonido ambiente de esta escena."
    )]
    [Range(0f, 1f)]
    [SerializeField]
    private float ambienceVolume = 0.18f;

    [Tooltip(
        "Nivel de SFX de esta escena, incluyendo UI, " +
        "page flips, decisiones y pasos."
    )]
    [Range(0f, 1f)]
    [SerializeField]
    private float sfxVolume = 0.85f;


    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "SceneAudio: No existe AudioManager."
            );

            return;
        }


        AudioManager.Instance.SetSceneMix(
            musicVolume,
            ambienceVolume,
            sfxVolume
        );


        ConfigureMusic();


        ConfigureAmbience();


        RestoreMusicAfterDeath();
    }


    private void ConfigureMusic()
    {

        if (keepPreviousMusic)
        {
            if (AudioManager.Instance.IsMusicPlaying())
            {
                return;
            }

            if (musicClip != null)
            {
                AudioManager.Instance.PlayMusic(
                    musicClip,
                    false
                );
            }

            return;
        }


        AudioManager.Instance.PlayMusic(
            musicClip,
            true
        );
    }


    private void ConfigureAmbience()
    {
        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }


    private void RestoreMusicAfterDeath()
    {
        if (!NarrativeState.RestoreMusicAfterDeath)
        {
            return;
        }

        AudioClip previousMusic =
            NarrativeState.MusicBeforeDeath;

        if (previousMusic == null)
        {
            NarrativeState.RestoreMusicAfterDeath =
                false;

            return;
        }


        AudioManager.Instance.PlayMusic(
            previousMusic,
            true
        );

        NarrativeState.RestoreMusicAfterDeath =
            false;

        NarrativeState.MusicBeforeDeath =
            null;
    }
}
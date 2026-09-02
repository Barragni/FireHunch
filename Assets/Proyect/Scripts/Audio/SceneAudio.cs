using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    // ============================================================
    // MÚSICA
    // ============================================================

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

    // ============================================================
    // AMBIENTE
    // ============================================================

    [Header("Ambiente")]
    [SerializeField]
    private AudioClip ambienceClip;

    // ============================================================
    // MEZCLA DE ESTA ESCENA
    // ============================================================

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

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "SceneAudio: No existe AudioManager."
            );

            return;
        }

        // ========================================================
        // MEZCLA DE LA ESCENA
        // ========================================================

        AudioManager.Instance.SetSceneMix(
            musicVolume,
            ambienceVolume,
            sfxVolume
        );

        // ========================================================
        // MÚSICA NORMAL DE LA ESCENA
        // ========================================================

        ConfigureMusic();

        // ========================================================
        // AMBIENTE
        // ========================================================

        ConfigureAmbience();

        // ========================================================
        // RESTAURACIÓN DESPUÉS DE MUERTE
        // ========================================================
        //
        // IMPORTANTE:
        //
        // Esto ocurre DESPUÉS de ConfigureMusic().
        //
        // Por lo tanto Keep Previous Music ya no puede impedir
        // la restauración de la música que estaba sonando antes
        // de morir.
        // ========================================================

        RestoreMusicAfterDeath();
    }

    // ============================================================
    // MÚSICA
    // ============================================================

    private void ConfigureMusic()
    {
        // --------------------------------------------------------
        // CONSERVAR MÚSICA ANTERIOR
        // --------------------------------------------------------

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

        // --------------------------------------------------------
        // USAR MÚSICA DE ESTA ESCENA
        // --------------------------------------------------------

        AudioManager.Instance.PlayMusic(
            musicClip,
            true
        );
    }

    // ============================================================
    // AMBIENTE
    // ============================================================

    private void ConfigureAmbience()
    {
        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }

    // ============================================================
    // RESTAURAR MÚSICA DESPUÉS DE MUERTE
    // ============================================================

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

        // ========================================================
        // RESTAURAR LA MÚSICA ORIGINAL
        // ========================================================
        //
        // restartEvenIfSame = true garantiza que si por alguna
        // razón SceneAudio dejó exactamente el mismo clip,
        // igualmente vuelva a comenzar.
        //
        // La música restaurada es la que estaba sonando antes
        // de la muerte.
        // ========================================================

        AudioManager.Instance.PlayMusic(
            previousMusic,
            true
        );

        // ========================================================
        // CONSUMIR LA ORDEN DE RESTAURACIÓN
        // ========================================================

        NarrativeState.RestoreMusicAfterDeath =
            false;

        NarrativeState.MusicBeforeDeath =
            null;
    }
}
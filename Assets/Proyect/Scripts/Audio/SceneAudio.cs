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
        // MÚSICA
        // ========================================================

        ConfigureMusic();

        // ========================================================
        // AMBIENTE
        // ========================================================

        ConfigureAmbience();
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
            // Si ya existe música reproduciéndose,
            // no tocar absolutamente nada.
            //
            // Continúa desde exactamente el mismo punto.
            if (AudioManager.Instance.IsMusicPlaying())
            {
                return;
            }

            // Si se pidió conservar música pero realmente
            // no había ninguna reproduciéndose, utilizar
            // Music Clip como respaldo.
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
        //
        // PlayMusic se encarga de detener la música anterior
        // antes de reproducir la nueva.
        //
        // Por lo tanto nunca existen dos músicas simultáneas.
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
        // Cada SceneAudio controla directamente
        // el ambiente correspondiente a su escena.
        //
        // Si ambienceClip es null, AudioManager
        // detendrá el ambiente anterior.

        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }
}
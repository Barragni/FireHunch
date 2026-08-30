using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    // ============================================================
    // TIPO DE ESCENA
    // ============================================================

    public enum SceneAudioType
    {
        Gameplay,
        Comic,
        Other
    }

    [Header("Tipo de escena")]
    [SerializeField]
    private SceneAudioType sceneType =
        SceneAudioType.Gameplay;

    // ============================================================
    // MÚSICA
    // ============================================================

    [Header("Música")]
    [SerializeField]
    private AudioClip musicClip;

    [Header("Comportamiento de música en Comic")]
    [Tooltip(
        "Si está activado, una escena Comic conserva la música " +
        "que venía reproduciéndose desde la escena anterior. " +
        "Si no había ninguna música sonando, utilizará Music Clip."
    )]
    [SerializeField]
    private bool keepPreviousMusicInComic = true;

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
        "Nivel del sonido ambiente de esta escena. " +
        "0.18 está pensado para una alarma de incendios " +
        "continua sin tapar música ni SFX."
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
                "No existe AudioManager en la escena Bootstrap."
            );

            return;
        }

        // ========================================================
        // APLICAR MEZCLA PROPIA DE ESTA ESCENA
        // ========================================================

        AudioManager.Instance.SetSceneMix(
            musicVolume,
            ambienceVolume,
            sfxVolume
        );

        // ========================================================
        // COMIC
        // ========================================================

        if (
            sceneType ==
            SceneAudioType.Comic
        )
        {
            ConfigureComicAudio();

            return;
        }

        // ========================================================
        // GAMEPLAY
        // ========================================================

        if (
            sceneType ==
            SceneAudioType.Gameplay
        )
        {
            ConfigureGameplayAudio();

            return;
        }

        // ========================================================
        // OTRAS ESCENAS
        // ========================================================

        ConfigureOtherAudio();
    }

    // ============================================================
    // GAMEPLAY
    // ============================================================

    private void ConfigureGameplayAudio()
    {
        // ========================================================
        // MÚSICA
        // ========================================================
        //
        // Gameplay SIEMPRE corta cualquier música anterior
        // y comienza la música asignada desde el principio.
        //
        // Esto impide que la música de un cómic sobreviva
        // accidentalmente al regresar al gameplay.
        // ========================================================

        AudioManager.Instance.PlayMusic(
            musicClip,
            true
        );

        // ========================================================
        // AMBIENTE
        // ========================================================

        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }

    // ============================================================
    // COMIC
    // ============================================================

    private void ConfigureComicAudio()
    {
        // ========================================================
        // AMBIENTE
        // ========================================================
        //
        // REGLA OBLIGATORIA:
        //
        // ningún sonido ambiente de Gameplay puede sobrevivir
        // a una escena Comic.
        //
        // Esto corta, por ejemplo, la alarma de incendios.
        // ========================================================

        AudioManager.Instance.StopAmbience();

        // ========================================================
        // MÚSICA
        // ========================================================

        if (keepPreviousMusicInComic)
        {
            // Si ya viene música desde Gameplay,
            // simplemente dejarla continuar exactamente
            // desde el punto donde estaba.
            if (
                AudioManager.Instance.IsMusicPlaying()
            )
            {
                return;
            }

            // Si llegamos al Comic sin música previa,
            // puede utilizarse Music Clip como respaldo.
            if (musicClip != null)
            {
                AudioManager.Instance.PlayMusic(
                    musicClip,
                    false
                );
            }

            return;
        }

        // Si deliberadamente desactivamos
        // Keep Previous Music In Comic,
        // el Comic controla su propia música.
        AudioManager.Instance.PlayMusic(
            musicClip,
            true
        );
    }

    // ============================================================
    // OTRAS ESCENAS
    // ============================================================

    private void ConfigureOtherAudio()
    {
        AudioManager.Instance.PlayMusic(
            musicClip,
            false
        );

        AudioManager.Instance.PlayAmbience(
            ambienceClip
        );
    }
}
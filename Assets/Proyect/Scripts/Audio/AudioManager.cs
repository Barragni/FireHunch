using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    // ============================================================
    // AUDIO SOURCES
    // ============================================================

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Source - Pasos")]
    [SerializeField] private AudioSource footstepSource;

    // ============================================================
    // UI SFX
    // ============================================================

    [Header("UI SFX")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip decisionOpen;
    [SerializeField] private AudioClip timerEnd;

    // ============================================================
    // COMIC SFX
    // ============================================================

    [Header("Comic SFX")]
    [SerializeField] private AudioClip pageFlip;

    // ============================================================
    // MÚSICA DE MUERTE
    // ============================================================

    [Header("Música de muerte")]

    [Tooltip(
        "Música que se reproduce cuando el jugador muere. " +
        "Reemplaza inmediatamente cualquier música que estuviera sonando."
    )]
    [SerializeField] private AudioClip deathMusic;

    [Tooltip(
        "Volumen independiente de la música de muerte. " +
        "1 = volumen completo, 0 = silencio."
    )]
    [Range(0f, 1f)]
    [SerializeField] private float deathMusicVolume = 0.5f;

    // ============================================================
    // PASOS DEL JUGADOR
    // ============================================================

    [Header("Pasos del jugador")]
    [SerializeField] private AudioClip footstepClip;

    [Range(0f, 1f)]
    [SerializeField] private float footstepVolume = 1f;

    // ============================================================
    // VOLUMEN CONFIGURADO POR EL JUGADOR
    // ============================================================

    public float MasterVolume { get; private set; } = 1f;

    public float MusicVolume { get; private set; } = 1f;

    public float SFXVolume { get; private set; } = 1f;

    // ============================================================
    // MEZCLA DE LA ESCENA ACTUAL
    // ============================================================

    private float sceneMusicVolume = 0.55f;

    private float sceneAmbienceVolume = 0.18f;

    private float sceneSFXVolume = 0.85f;

    public float SceneMusicVolume =>
        sceneMusicVolume;

    public float SceneAmbienceVolume =>
        sceneAmbienceVolume;

    public float SceneSFXVolume =>
        sceneSFXVolume;

    // ============================================================
    // ESTADO DE MÚSICA DE MUERTE
    // ============================================================

    private bool isDeathMusicPlaying;

    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            SetupAudioSources();

            SetupFootstepSource();

            LoadVolumes();

            ResetSceneMix();

            ApplyVolumes();

            StopAllAudio();

            SceneManager.sceneLoaded +=
                OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;
        }
    }

    // ============================================================
    // CONFIGURAR AUDIO SOURCES
    // ============================================================

    private void SetupAudioSources()
    {
        if (musicSource != null)
        {
            musicSource.playOnAwake = false;

            musicSource.loop = true;
        }

        if (ambienceSource != null)
        {
            ambienceSource.playOnAwake = false;

            ambienceSource.loop = true;
        }

        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;

            sfxSource.loop = false;
        }
    }

    // ============================================================
    // CONFIGURAR SOURCE DE PASOS
    // ============================================================

    private void SetupFootstepSource()
    {
        // Si no se asignó manualmente en el Inspector,
        // crear uno automáticamente.

        if (footstepSource == null)
        {
            footstepSource =
                gameObject.AddComponent<AudioSource>();
        }

        footstepSource.playOnAwake = false;

        footstepSource.loop = false;

        footstepSource.spatialBlend = 0f;
    }

    // ============================================================
    // CAMBIO DE ESCENA
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        // ========================================================
        // IMPORTANTE
        // ========================================================
        //
        // NO detener la música aquí.
        //
        // SceneAudio decide posteriormente qué música debe sonar.
        //
        // Esto permite que Keep Previous Music funcione
        // correctamente entre escenas.
        // ========================================================

        StopSFX();

        StopFootstep();

        // Restablecer la mezcla base antes de que
        // SceneAudio configure la nueva escena.

        ResetSceneMix();
    }

    // ============================================================
    // MEZCLA POR ESCENA
    // ============================================================

    /// <summary>
    /// Configura los niveles propios de la escena actual.
    ///
    /// Estos valores se multiplican por los ajustes generales
    /// seleccionados por el jugador.
    /// </summary>
    public void SetSceneMix(
        float musicVolume,
        float ambienceVolume,
        float sfxVolume
    )
    {
        sceneMusicVolume =
            Mathf.Clamp01(
                musicVolume
            );

        sceneAmbienceVolume =
            Mathf.Clamp01(
                ambienceVolume
            );

        sceneSFXVolume =
            Mathf.Clamp01(
                sfxVolume
            );

        ApplyVolumes();
    }

    private void ResetSceneMix()
    {
        // ========================================================
        // MEZCLA BASE
        // ========================================================

        sceneMusicVolume = 0.55f;

        sceneAmbienceVolume = 0.18f;

        sceneSFXVolume = 0.85f;

        ApplyVolumes();
    }

    // ============================================================
    // VOLUMEN GENERAL
    // ============================================================

    public void SetMasterVolume(float value)
    {
        MasterVolume =
            Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(
            MASTER_KEY,
            MasterVolume
        );

        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume =
            Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(
            MUSIC_KEY,
            MusicVolume
        );

        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume =
            Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(
            SFX_KEY,
            SFXVolume
        );

        PlayerPrefs.Save();
    }

    public void ResetVolumes()
    {
        MasterVolume = 1f;

        MusicVolume = 1f;

        SFXVolume = 1f;

        ApplyVolumes();

        PlayerPrefs.SetFloat(
            MASTER_KEY,
            MasterVolume
        );

        PlayerPrefs.SetFloat(
            MUSIC_KEY,
            MusicVolume
        );

        PlayerPrefs.SetFloat(
            SFX_KEY,
            SFXVolume
        );

        PlayerPrefs.Save();
    }

    // ============================================================
    // CARGAR VOLUMEN
    // ============================================================

    private void LoadVolumes()
    {
        MasterVolume =
            PlayerPrefs.GetFloat(
                MASTER_KEY,
                1f
            );

        MusicVolume =
            PlayerPrefs.GetFloat(
                MUSIC_KEY,
                1f
            );

        SFXVolume =
            PlayerPrefs.GetFloat(
                SFX_KEY,
                1f
            );
    }

    // ============================================================
    // APLICAR VOLUMEN
    // ============================================================

    private void ApplyVolumes()
    {
        // ========================================================
        // MÚSICA
        // ========================================================

        if (musicSource != null)
        {
            if (isDeathMusicPlaying)
            {
                // La música de muerte tiene su propio volumen.

                musicSource.volume =
                    MasterVolume *
                    MusicVolume *
                    sceneMusicVolume *
                    deathMusicVolume;
            }
            else
            {
                // Música normal.

                musicSource.volume =
                    MasterVolume *
                    MusicVolume *
                    sceneMusicVolume;
            }
        }

        // ========================================================
        // AMBIENTE
        // ========================================================

        if (ambienceSource != null)
        {
            ambienceSource.volume =
                MasterVolume *
                SFXVolume *
                sceneAmbienceVolume;
        }

        // ========================================================
        // SFX
        // ========================================================

        if (sfxSource != null)
        {
            sfxSource.volume =
                MasterVolume *
                SFXVolume *
                sceneSFXVolume;
        }

        // ========================================================
        // PASOS
        // ========================================================

        if (footstepSource != null)
        {
            footstepSource.volume =
                MasterVolume *
                SFXVolume *
                sceneSFXVolume *
                footstepVolume;
        }
    }

    // ============================================================
    // RESET GENERAL DE AUDIO
    // ============================================================

    public void StopAllAudio()
    {
        StopMusic();

        StopAmbience();

        StopSFX();

        StopFootstep();
    }

    // ============================================================
    // MUSIC
    // ============================================================

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();

        musicSource.clip = null;

        isDeathMusicPlaying = false;

        ApplyVolumes();
    }

    /// <summary>
    /// Reproduce música.
    ///
    /// Si restartEvenIfSame es false y ya está sonando
    /// exactamente el mismo clip, continúa sin reiniciarlo.
    ///
    /// Si restartEvenIfSame es true, el clip comienza
    /// obligatoriamente desde el principio.
    /// </summary>
    public void PlayMusic(
        AudioClip clip,
        bool restartEvenIfSame = false
    )
    {
        if (musicSource == null)
            return;

        if (clip == null)
        {
            StopMusic();

            return;
        }

        if (
            !restartEvenIfSame &&
            musicSource.clip == clip &&
            musicSource.isPlaying &&
            !isDeathMusicPlaying
        )
        {
            return;
        }

        // ========================================================
        // YA NO ES MÚSICA DE MUERTE
        // ========================================================

        isDeathMusicPlaying = false;

        // ========================================================
        // DETENER CUALQUIER MÚSICA ANTERIOR
        // ========================================================

        musicSource.Stop();

        // ========================================================
        // ASIGNAR NUEVA MÚSICA
        // ========================================================

        musicSource.clip = clip;

        musicSource.loop = true;

        ApplyVolumes();

        musicSource.Play();
    }

    // ============================================================
    // MÚSICA DE MUERTE
    // ============================================================

    /// <summary>
    /// Detiene inmediatamente la música que estuviera sonando
    /// y reproduce la música específica de muerte.
    ///
    /// Utiliza el mismo AudioSource de música general, por lo que
    /// nunca se reproducen simultáneamente la música normal y
    /// la música de muerte.
    ///
    /// Death Music Volume permite ajustar exclusivamente
    /// el volumen de esta música desde el Inspector.
    /// </summary>
    public void PlayDeathMusic()
    {
        if (musicSource == null)
            return;

        if (deathMusic == null)
        {
            Debug.LogWarning(
                "AudioManager: No se asignó una música en Death Music."
            );

            return;
        }

        // ========================================================
        // DETENER MÚSICA ANTERIOR
        // ========================================================

        musicSource.Stop();

        // ========================================================
        // MARCAR MÚSICA DE MUERTE
        // ========================================================

        isDeathMusicPlaying = true;

        // ========================================================
        // REPRODUCIR MÚSICA DE MUERTE
        // ========================================================

        musicSource.clip =
            deathMusic;

        musicSource.loop = true;

        ApplyVolumes();

        musicSource.Play();
    }

    // ============================================================
    // CONSULTAR MÚSICA
    // ============================================================

    /// <summary>
    /// Devuelve true si actualmente existe
    /// una música reproduciéndose.
    /// </summary>
    public bool IsMusicPlaying()
    {
        return
            musicSource != null &&
            musicSource.clip != null &&
            musicSource.isPlaying;
    }

    /// <summary>
    /// Devuelve el AudioClip que actualmente tiene
    /// asignado el AudioSource de música.
    /// </summary>
    public AudioClip GetCurrentMusic()
    {
        if (musicSource == null)
        {
            return null;
        }

        return musicSource.clip;
    }

    // ============================================================
    // AMBIENCE
    // ============================================================

    public void StopAmbience()
    {
        if (ambienceSource == null)
            return;

        ambienceSource.Stop();

        ambienceSource.clip = null;
    }

    public void PlayAmbience(
        AudioClip clip
    )
    {
        if (ambienceSource == null)
            return;

        if (clip == null)
        {
            StopAmbience();

            return;
        }

        if (
            ambienceSource.clip == clip &&
            ambienceSource.isPlaying
        )
        {
            return;
        }

        ambienceSource.Stop();

        ambienceSource.clip = clip;

        ambienceSource.loop = true;

        ApplyVolumes();

        ambienceSource.Play();
    }

    // ============================================================
    // SFX
    // ============================================================

    public void StopSFX()
    {
        if (sfxSource == null)
            return;

        sfxSource.Stop();

        sfxSource.clip = null;
    }

    public void PlaySFX(
        AudioClip clip
    )
    {
        if (
            clip == null ||
            sfxSource == null
        )
        {
            return;
        }

        ApplyVolumes();

        sfxSource.PlayOneShot(
            clip
        );
    }

    // ============================================================
    // PASOS
    // ============================================================

    /// <summary>
    /// Comienza el sonido de pasos.
    ///
    /// El clip NO se reinicia mientras ya esté sonando.
    /// </summary>
    public void PlayFootstep()
    {
        if (
            footstepSource == null ||
            footstepClip == null
        )
        {
            return;
        }

        footstepSource.clip =
            footstepClip;

        footstepSource.loop = true;

        footstepSource.volume =
            MasterVolume *
            SFXVolume *
            sceneSFXVolume *
            footstepVolume;

        if (!footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
    }

    public void StopFootstep()
    {
        if (footstepSource == null)
            return;

        footstepSource.Stop();

        footstepSource.clip = null;
    }

    // ============================================================
    // UI
    // ============================================================

    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    public void PlayDecisionOpen()
    {
        PlaySFX(decisionOpen);
    }

    public void PlayTimerEnd()
    {
        PlaySFX(timerEnd);
    }

    // ============================================================
    // COMIC
    // ============================================================

    public void PlayPageFlip()
    {
        PlaySFX(pageFlip);
    }
}
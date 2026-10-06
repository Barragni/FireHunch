using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";


    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Source - Pasos")]
    [SerializeField] private AudioSource footstepSource;


    [Header("UI SFX")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip decisionOpen;
    [SerializeField] private AudioClip timerEnd;


    [Header("Diálogo")]
    [SerializeField] private AudioClip dialogueBlip;

    [Tooltip(
        "Volumen independiente del sonido que se reproduce " +
        "cada vez que aparece una nueva línea de diálogo."
    )]
    [Range(0f, 1f)]
    [SerializeField] private float dialogueVolume = 1f;


    [Header("Comic SFX")]
    [SerializeField] private AudioClip pageFlip;


    [Header("Música de muerte")]

    [Tooltip(
        "Música que se reproduce cuando el jugador muere."
    )]
    [SerializeField] private AudioClip deathMusic;

    [Tooltip(
        "Volumen máximo de la música de muerte."
    )]
    [Range(0f, 1f)]
    [SerializeField] private float deathMusicVolume = 0.5f;

    [Tooltip(
        "Tiempo que tarda la música de muerte en subir " +
        "desde silencio hasta su volumen configurado."
    )]
    [SerializeField] private float deathMusicFadeInDuration = 1.5f;


    [Header("Pasos del jugador")]
    [SerializeField] private AudioClip footstepClip;

    [Range(0f, 1f)]
    [SerializeField] private float footstepVolume = 1f;


    public float MasterVolume { get; private set; } = 1f;

    public float MusicVolume { get; private set; } = 1f;

    public float SFXVolume { get; private set; } = 1f;


    private float sceneMusicVolume = 0.55f;

    private float sceneAmbienceVolume = 0.18f;

    private float sceneSFXVolume = 0.85f;

    public float SceneMusicVolume =>
        sceneMusicVolume;

    public float SceneAmbienceVolume =>
        sceneAmbienceVolume;

    public float SceneSFXVolume =>
        sceneSFXVolume;


    private bool isDeathMusicPlaying;

    private Coroutine deathMusicFadeCoroutine;


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


    private void SetupFootstepSource()
    {
        if (footstepSource == null)
        {
            footstepSource =
                gameObject.AddComponent<AudioSource>();
        }

        footstepSource.playOnAwake = false;

        footstepSource.loop = false;

        footstepSource.spatialBlend = 0f;
    }


    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        StopSFX();

        StopFootstep();

        ResetSceneMix();
    }


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
        sceneMusicVolume = 0.55f;

        sceneAmbienceVolume = 0.18f;

        sceneSFXVolume = 0.85f;

        ApplyVolumes();
    }


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


    private void ApplyVolumes()
    {
        if (musicSource != null)
        {
            if (isDeathMusicPlaying)
            {
                musicSource.volume =
                    MasterVolume *
                    MusicVolume *
                    sceneMusicVolume *
                    deathMusicVolume;
            }
            else
            {
                musicSource.volume =
                    MasterVolume *
                    MusicVolume *
                    sceneMusicVolume;
            }
        }

        if (ambienceSource != null)
        {
            ambienceSource.volume =
                MasterVolume *
                SFXVolume *
                sceneAmbienceVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume =
                MasterVolume *
                SFXVolume *
                sceneSFXVolume;
        }

        if (footstepSource != null)
        {
            footstepSource.volume =
                MasterVolume *
                SFXVolume *
                sceneSFXVolume *
                footstepVolume;
        }
    }


    public void StopAllAudio()
    {
        StopMusic();

        StopAmbience();

        StopSFX();

        StopFootstep();
    }


    public void StopMusic()
    {
        if (deathMusicFadeCoroutine != null)
        {
            StopCoroutine(
                deathMusicFadeCoroutine
            );

            deathMusicFadeCoroutine = null;
        }

        if (musicSource == null)
            return;

        musicSource.Stop();

        musicSource.clip = null;

        isDeathMusicPlaying = false;

        ApplyVolumes();
    }


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

        if (deathMusicFadeCoroutine != null)
        {
            StopCoroutine(
                deathMusicFadeCoroutine
            );

            deathMusicFadeCoroutine = null;
        }

        isDeathMusicPlaying = false;

        if (
            !restartEvenIfSame &&
            musicSource.clip == clip &&
            musicSource.isPlaying
        )
        {
            return;
        }

        musicSource.Stop();

        musicSource.clip = clip;

        musicSource.loop = true;

        ApplyVolumes();

        musicSource.Play();
    }


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

        if (deathMusicFadeCoroutine != null)
        {
            StopCoroutine(
                deathMusicFadeCoroutine
            );

            deathMusicFadeCoroutine = null;
        }

        musicSource.Stop();

        musicSource.clip = null;

        isDeathMusicPlaying = true;

        musicSource.clip =
            deathMusic;

        musicSource.loop = true;

        musicSource.volume = 0f;

        musicSource.Play();

        deathMusicFadeCoroutine =
            StartCoroutine(
                FadeInDeathMusic()
            );
    }


    private IEnumerator FadeInDeathMusic()
    {
        float duration =
            Mathf.Max(
                0f,
                deathMusicFadeInDuration
            );

        float targetVolume =
            MasterVolume *
            MusicVolume *
            sceneMusicVolume *
            deathMusicVolume;

        if (duration <= 0f)
        {
            musicSource.volume =
                targetVolume;

            deathMusicFadeCoroutine = null;

            yield break;
        }

        float elapsed = 0f;

        while (
            elapsed <
            duration
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );

            musicSource.volume =
                Mathf.Lerp(
                    0f,
                    targetVolume,
                    progress
                );

            yield return null;
        }

        musicSource.volume =
            targetVolume;

        deathMusicFadeCoroutine = null;
    }


    public bool IsMusicPlaying()
    {
        return
            musicSource != null &&
            musicSource.clip != null &&
            musicSource.isPlaying;
    }

    public AudioClip GetCurrentMusic()
    {
        if (musicSource == null)
        {
            return null;
        }

        return musicSource.clip;
    }


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


    public void PlayDialogueBlip()
    {
        if (
            dialogueBlip == null ||
            sfxSource == null
        )
        {
            return;
        }

        ApplyVolumes();

        sfxSource.PlayOneShot(
            dialogueBlip,
            dialogueVolume
        );
    }


    public void PlayPageFlip()
    {
        PlaySFX(pageFlip);
    }
}
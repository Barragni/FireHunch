using UnityEngine;

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

    [Header("UI SFX")]
    [SerializeField] private AudioClip buttonClick;

    [SerializeField] private AudioClip decisionOpen;

    [SerializeField] private AudioClip timerEnd;

    [Header("Comic SFX")]
    [SerializeField] private AudioClip pageFlip;

    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadVolumes();
            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Volumen

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(MASTER_KEY, MasterVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);

        ApplyVolumes();

        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);
        PlayerPrefs.Save();
    }

    public void ResetVolumes()
    {
        MasterVolume = 1f;
        MusicVolume = 1f;
        SFXVolume = 1f;

        ApplyVolumes();

        PlayerPrefs.SetFloat(MASTER_KEY, MasterVolume);
        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);

        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        MasterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = MasterVolume * MusicVolume;

        if (ambienceSource != null)
            ambienceSource.volume = MasterVolume * SFXVolume;

        if (sfxSource != null)
            sfxSource.volume = MasterVolume * SFXVolume;
    }

    #endregion

    #region Music

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (musicSource.clip == clip &&
            musicSource.isPlaying)
            return;

        musicSource.Stop();

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    #endregion

    #region Ambience

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null)
            return;

        if (ambienceSource.clip == clip &&
            ambienceSource.isPlaying)
            return;

        ambienceSource.Stop();

        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    #endregion

    #region Generic SFX

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    #endregion

    #region UI

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

    #endregion

    #region Comic

    public void PlayPageFlip()
    {
        PlaySFX(pageFlip);
    }

    #endregion
}
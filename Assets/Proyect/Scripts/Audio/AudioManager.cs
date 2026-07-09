using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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

    #region Music

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

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
        {
            return;
        }

        if (ambienceSource.clip == clip &&
            ambienceSource.isPlaying)
        {
            return;
        }

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
        {
            return;
        }

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
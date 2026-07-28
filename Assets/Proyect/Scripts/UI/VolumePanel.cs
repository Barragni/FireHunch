using UnityEngine;
using UnityEngine.UI;

public class VolumePanel : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Botones")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button defaultButton;

    private void Start()
    {
        // Cargar valores actuales
        masterSlider.SetValueWithoutNotify(AudioManager.Instance.MasterVolume);
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);

        // Eventos sliders
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        // Botones
        acceptButton.onClick.AddListener(OnAccept);
        defaultButton.onClick.AddListener(OnDefault);
    }

    private void OnDestroy()
    {
        masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);

        acceptButton.onClick.RemoveListener(OnAccept);
        defaultButton.onClick.RemoveListener(OnDefault);
    }

    private void OnMasterChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnDefault()
    {
        AudioManager.Instance.ResetVolumes();

        masterSlider.SetValueWithoutNotify(AudioManager.Instance.MasterVolume);
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
    }

    private void OnAccept()
    {
        gameObject.SetActive(false);
    }
}
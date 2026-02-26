using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public sealed class AudioOptionsManager : MonoBehaviour
{
    private const float MinLinearVolume = 0.0001f;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";
    [SerializeField] private string sfxWeaponsVolumeParam = "SFXWeaponsVolume";

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        BindSlider(masterSlider, SetMasterVolume);
        BindSlider(musicSlider, SetMusicVolume);
        BindSlider(sfxSlider, SetSfxVolume);

        RefreshFromCurrentSliderValues();
    }

    private void OnDisable()
    {
        UnbindSlider(masterSlider, SetMasterVolume);
        UnbindSlider(musicSlider, SetMusicVolume);
        UnbindSlider(sfxSlider, SetSfxVolume);
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume(masterVolumeParam, value);
    }

    public void SetMusicVolume(float value)
    {
        SetMixerVolume(musicVolumeParam, value);
    }

    public void SetSfxVolume(float value)
    {
        SetMixerVolume(sfxVolumeParam, value);
        SetMixerVolume(sfxWeaponsVolumeParam, value);
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mixer == null || string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }

        float clamped = Mathf.Clamp(sliderValue, MinLinearVolume, 1f);
        float dB = Mathf.Log10(clamped) * 20f;

        bool applied = mixer.SetFloat(parameterName, dB);
        if (!applied)
        {
            Debug.LogWarning($"AudioOptionsManager could not set '{parameterName}'. Make sure it is exposed in the AudioMixer.", this);
        }
    }

    private void RefreshFromCurrentSliderValues()
    {
        if (masterSlider) SetMasterVolume(masterSlider.value);
        if (musicSlider) SetMusicVolume(musicSlider.value);
        if (sfxSlider) SetSfxVolume(sfxSlider.value);
    }

    private static void BindSlider(Slider slider, UnityEngine.Events.UnityAction<float> handler)
    {
        if (slider == null) return;
        slider.onValueChanged.AddListener(handler);
    }

    private static void UnbindSlider(Slider slider, UnityEngine.Events.UnityAction<float> handler)
    {
        if (slider == null) return;
        slider.onValueChanged.RemoveListener(handler);
    }
}

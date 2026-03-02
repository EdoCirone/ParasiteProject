using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public sealed class AudioOptionsManager : MonoBehaviour
{
    public static AudioOptionsManager Instance { get; private set; }
    private const float MinLinearVolume = 0.0001f;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";
    [SerializeField] private string sfxWeaponsVolumeParam = "SFXWeaponsVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float value) => SetMixerVolume(masterVolumeParam, value);
    public void SetMusicVolume(float value) => SetMixerVolume(musicVolumeParam, value);
    public void SetSfxVolume(float value)
    {
        SetMixerVolume(sfxVolumeParam, value);
        SetMixerVolume(sfxWeaponsVolumeParam, value);
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mixer == null || string.IsNullOrWhiteSpace(parameterName)) return;

        float clamped = Mathf.Clamp(sliderValue, MinLinearVolume, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        mixer.SetFloat(parameterName, dB);
    }
}
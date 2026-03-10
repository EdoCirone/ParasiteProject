using UnityEngine;
using UnityEngine.Audio;

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

    // Optional: You can use PlayerPrefs to save and load volume settings using the keys defined below.
    private const string MasterVolumeKey = "Audio_Master";
    private const string MusicVolumeKey = "Audio_Music";
    private const string SfxVolumeKey = "Audio_SFX";

    public bool IsInitialized { get; private set; }

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

    private void Start()
    {
        LoadVolume();
        IsInitialized = true;
    }


    public void SetMasterVolume(float value) => SetMasterVolume(value, true);
    public void SetMusicVolume(float value) => SetMusicVolume(value, true);
    public void SetSfxVolume(float value) => SetSfxVolume(value, true);

    public float GetSavedMasterVolume() => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
    public float GetSavedMusicVolume() => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    public float GetSavedSfxVolume() => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

    private void SetMasterVolume(float value, bool save)
    {
        value = Mathf.Clamp01(value);
        SetMixerVolume(masterVolumeParam, value);
        if (save)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }


    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mixer == null || string.IsNullOrWhiteSpace(parameterName)) return;

        float clamped = Mathf.Clamp(sliderValue, MinLinearVolume, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        mixer.SetFloat(parameterName, dB);
    }

    private void SetMusicVolume(float value, bool save)
    {
        value = Mathf.Clamp01(value);
        SetMixerVolume(musicVolumeParam, value);
        if (save)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    private void SetSfxVolume(float value, bool save)
    {
        value = Mathf.Clamp01(value);
        SetMixerVolume(sfxVolumeParam, value);
        SetMixerVolume(sfxWeaponsVolumeParam, value);
        if (save)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    private void LoadVolume()
    {
        float master = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float music = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        SetMasterVolume(master, false);
        SetMusicVolume(music, false);
        SetSfxVolume(sfx, false);
    }
}
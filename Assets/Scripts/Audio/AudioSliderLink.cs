using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSliderLink : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }
    public VolumeType type;

    private Slider slider;

    private IEnumerator Start()
    {
        slider = GetComponent<Slider>();

        while (AudioOptionsManager.Instance == null || !AudioOptionsManager.Instance.IsInitialized)
            yield return null;

        slider.onValueChanged.RemoveListener(OnChanged);

        switch (type)
        {
            case VolumeType.Master:
                slider.value = AudioOptionsManager.Instance.GetSavedMasterVolume();
                break;
            case VolumeType.Music:
                slider.value = AudioOptionsManager.Instance.GetSavedMusicVolume();
                break;
            case VolumeType.SFX:
                slider.value = AudioOptionsManager.Instance.GetSavedSfxVolume();
                break;
        }

        slider.onValueChanged.AddListener(OnChanged);
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnChanged);
        }
    }

    private void OnChanged(float value)
    {
        if (AudioOptionsManager.Instance == null) return;

        switch (type)
        {
            case VolumeType.Master: AudioOptionsManager.Instance.SetMasterVolume(value); break;
            case VolumeType.Music: AudioOptionsManager.Instance.SetMusicVolume(value); break;
            case VolumeType.SFX: AudioOptionsManager.Instance.SetSfxVolume(value); break;
        }
    }
}
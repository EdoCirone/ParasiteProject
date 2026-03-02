using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSliderLink : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }
    public VolumeType type;

    private void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnChanged);

        // Inizializza il volume corrente (opzionale)
        OnChanged(slider.value);
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
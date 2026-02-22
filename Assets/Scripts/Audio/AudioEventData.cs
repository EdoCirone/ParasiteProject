using UnityEngine;

public enum GameAudioEventType
{
    Hit,
    Shot,
    Death,
    Jump,
    LowHealth
}

public enum AudioRoute
{
    SFX_Weapons,
    SFX_Impacts
}

[CreateAssetMenu(fileName = "AudioEventData", menuName = "Audio/Audio Event Data")]
public sealed class AudioEventData : ScriptableObject
{
    [Header("Event")]
    [SerializeField] private GameAudioEventType eventType;
    [SerializeField] private AudioRoute route = AudioRoute.SFX_Impacts;

    [Header("Clip Variations")]
    [SerializeField] private AudioClip[] clips;

    [Header("Playback")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private bool spatial = true;

    public GameAudioEventType EventType => eventType;
    public AudioRoute Route => route;
    public float Volume => volume;
    public bool Spatial => spatial;

    public bool TryGetRandomClip(out AudioClip clip)
    {
        clip = null;
        if (clips == null || clips.Length == 0)
        {
            return false;
        }

        int index = Random.Range(0, clips.Length);
        clip = clips[index];
        return clip != null;
    }
}

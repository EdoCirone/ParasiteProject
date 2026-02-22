using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public sealed class AudioManager : MonoBehaviour
{
    private struct ActiveVoice
    {
        public AudioSource Source;
        public float EndTime;
    }

    public static AudioManager Instance { get; private set; }

    [Header("Pool")]
    [SerializeField, Min(8)] private int defaultCapacity = 32;
    [SerializeField, Min(8)] private int maxPoolSize = 128;

    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup sfxWeaponsGroup;
    [SerializeField] private AudioMixerGroup sfxImpactsGroup;

    [Header("Pitch Randomization")]
    [SerializeField] private float randomPitchMin = 0.95f;
    [SerializeField] private float randomPitchMax = 1.05f;

    private ObjectPool<AudioSource> voicePool;
    private readonly List<ActiveVoice> activeVoices = new List<ActiveVoice>(128);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);

        voicePool = new ObjectPool<AudioSource>(CreateVoice, OnGetVoice, OnReleaseVoice, OnDestroyVoice, false, defaultCapacity, maxPoolSize);
    }

    private void Update()
    {
        float now = Time.unscaledTime;
        for (int i = activeVoices.Count - 1; i >= 0; i--)
        {
            if (activeVoices[i].EndTime > now)
            {
                continue;
            }

            AudioSource source = activeVoices[i].Source;
            activeVoices.RemoveAt(i);
            ReleaseVoice(source);
        }
    }

    public void PlaySound(AudioEventData data, Vector3 position)
    {
        if (data == null || !data.TryGetRandomClip(out AudioClip clip))
        {
            return;
        }

        AudioSource voice = AcquireVoice();
        voice.transform.position = position;
        voice.outputAudioMixerGroup = ResolveRoute(data.Route);
        voice.spatialBlend = data.Spatial ? 1f : 0f;
        voice.pitch = Random.Range(randomPitchMin, randomPitchMax);
        voice.volume = data.Volume;
        voice.clip = clip;
        voice.Play();

        ActiveVoice activeVoice = new ActiveVoice
        {
            Source = voice,
            EndTime = Time.unscaledTime + (clip.length / Mathf.Max(0.01f, voice.pitch))
        };

        activeVoices.Add(activeVoice);
    }

    private AudioSource AcquireVoice()
    {
        AudioSource source;

        if (voicePool.CountInactive > 0)
        {
            source = voicePool.Get();
            return source;
        }

        if (activeVoices.Count >= maxPoolSize && activeVoices.Count > 0)
        {
            source = activeVoices[0].Source;
            activeVoices.RemoveAt(0);
            source.Stop();
            return source;
        }

        source = voicePool.Get();
        return source;
    }

    private void ReleaseVoice(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        voicePool.Release(source);
    }


    private AudioMixerGroup ResolveRoute(AudioRoute route)
    {
        switch (route)
        {
            case AudioRoute.SFX_Weapons:
                return sfxWeaponsGroup;
            case AudioRoute.SFX_Impacts:
            default:
                return sfxImpactsGroup;
        }
    }

    private AudioSource CreateVoice()
    {
        GameObject voiceObject = new GameObject("PooledAudioVoice");
        voiceObject.transform.SetParent(transform, false);

        AudioSource source = voiceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 2f;
        source.maxDistance = 35f;
        return source;
    }

    private void OnGetVoice(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private void OnReleaseVoice(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.outputAudioMixerGroup = null;
        source.gameObject.SetActive(false);
    }

    private void OnDestroyVoice(AudioSource source)
    {
        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }
}

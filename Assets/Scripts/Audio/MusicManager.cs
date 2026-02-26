using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public sealed class MusicManager : MonoBehaviour
{
    public enum MusicState
    {
        MainMenu,
        Gameplay
    }

    public static MusicManager Instance { get; private set; }

    [Header("Routing")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [Header("Tracks")]
    [SerializeField] private AudioClip mainMenuTrack;
    [SerializeField] private AudioClip gameplayTrack;

    [Header("Crossfade")]
    [SerializeField, Min(0.1f)] private float crossfadeDuration = 2.5f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;

    private AudioSource[] musicSources;
    private int activeSourceIndex;
    private Coroutine crossfadeRoutine;
    private MusicState? currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSources();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ApplyMusicForScene(SceneManager.GetActiveScene());
    }

    public void PlayMusic(MusicState state)
    {
        if (currentState == state)
        {
            return;
        }

        currentState = state;
        AudioClip clip = GetClipForState(state);
        if (!clip)
        {
            return;
        }

        CrossfadeTo(clip);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicForScene(scene);
    }

    private void ApplyMusicForScene(Scene scene)
    {
        string sceneName = scene.name.ToLowerInvariant();
        bool isMenu = sceneName.Contains("menu");
        PlayMusic(isMenu ? MusicState.MainMenu : MusicState.Gameplay);
    }

    private AudioClip GetClipForState(MusicState state)
    {
        return state == MusicState.MainMenu ? mainMenuTrack : gameplayTrack;
    }

    private void CrossfadeTo(AudioClip nextClip)
    {
        if (crossfadeRoutine != null)
        {
            StopCoroutine(crossfadeRoutine);
        }

        int nextSourceIndex = 1 - activeSourceIndex;
        AudioSource nextSource = musicSources[nextSourceIndex];
        nextSource.clip = nextClip;
        nextSource.volume = 0f;
        nextSource.Play();

        crossfadeRoutine = StartCoroutine(CrossfadeRoutine(musicSources[activeSourceIndex], nextSource, crossfadeDuration));
        activeSourceIndex = nextSourceIndex;
    }

    private IEnumerator CrossfadeRoutine(AudioSource from, AudioSource to, float duration)
    {
        if (duration <= 0f)
        {
            if (from) from.Stop();
            if (to) to.volume = musicVolume;
            yield break;
        }

        float elapsed = 0f;
        float fromStart = from ? from.volume : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (from)
            {
                from.volume = Mathf.Lerp(fromStart, 0f, t);
            }

            if (to)
            {
                to.volume = Mathf.Lerp(0f, musicVolume, t);
            }

            yield return null;
        }

        if (from)
        {
            from.volume = 0f;
            from.Stop();
        }

        if (to)
        {
            to.volume = musicVolume;
        }

        crossfadeRoutine = null;
    }

    private void InitializeSources()
    {
        musicSources = new AudioSource[2];
        for (int i = 0; i < musicSources.Length; i++)
        {
            GameObject sourceObject = new GameObject($"MusicSource_{i}");
            sourceObject.transform.SetParent(transform, false);

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            source.volume = 0f;
            source.outputAudioMixerGroup = musicMixerGroup;

            musicSources[i] = source;
        }

        activeSourceIndex = 0;
    }
}

# Event-Driven Audio System Plan (ParasiteProject)

## 1) Combat Audio Pipeline

### Hit SFX (Colpo Ricevuto)
- Trigger authority is on confirmed damage, not collision contact.
- Implement `IDamageable.TakeDamage(in DamageContext context)` on entities.
- The health component calls `AudioEvents.RaiseHit(...)` only when damage is effectively applied (ignores invulnerability, shields, or zero-damage hits).
- Route hit events to `SFX_Impacts` mixer group.

### Shot SFX (Colpo Sparato)
- Weapon-authoritative playback: trigger only on successful fire (`canFire`, ammo available, projectile spawned).
- Use pooled voices (`AudioSource[]`) to avoid runtime allocations and handle high RPM bursts.
- Apply pitch randomization between `0.95f` and `1.05f` (±5%) per shot to reduce repetition.
- Optional per-weapon event throttling for ultra-fast builds (e.g., max 1 SFX every 20-35 ms per emitter).
- Route to `SFX_Weapons` mixer group.

### Death SFX (Morte)
- Single-gate event in health component:
  - if `currentHp <= 0 && !isDead`, set `isDead = true`, fire `OnDeath`, play death SFX.
- Never gate death sound on animation event.
- Route to `SFX_Impacts` (or a `SFX_Deaths` child bus if added later).

## 2) Environmental & Feedback

### Ambience Loop
- Maintain one singleton ambience player (`DontDestroyOnLoad`) tied to `GameStateManager` state transitions.
- Crossfade in/out over `0.1s-0.5s` using coroutine or tween.
- Keep loop clip and volume independent from combat SFX.

### Leap SFX (Salto Cadavere)
- Target total duration: `250-450 ms`.
- Layer recipe:
  1. **Transient attack**: low thump `80-180 Hz`.
  2. **Whoosh body**: band-pass noise `1.2-3.5 kHz` with slight pitch contour.
  3. **Optional land tail**: short impact for readability.
- Route to `SFX_Impacts`.

### Low Health Alert (Premorte)
- Core oscillator/clip emphasis between `700-1100 Hz` for audibility.
- Pulse cadence tightens as HP decreases.
- Route to `UI_Alerts`.

## 3) Mix Architecture & Ducking

### Mixer Hierarchy
- `Master`
  - `Music`
  - `Ambience`
  - `SFX_Weapons`
  - `SFX_Impacts`
  - `UI_Alerts`

### Dynamics Policy
- Sidechain ducking target: `Ambience` only.
- Sidechain trigger source: `UI_Alerts` only (Low HP alert).
- Keep ambience around `-8 dB` to `-14 dB` relative to weapon bus during combat.
- Do not duck ambience from every shot/hit to avoid pumping in dense fights.

---

## Snippet A: SoundManager voice pooling (shots/hits/deaths)

```csharp
using UnityEngine;
using UnityEngine.Audio;

public enum AudioBusType { Weapons, Impacts, UIAlerts, Ambience }

public sealed class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private int poolSize = 32;
    [SerializeField] private AudioMixerGroup weaponsGroup;
    [SerializeField] private AudioMixerGroup impactsGroup;
    [SerializeField] private AudioMixerGroup uiAlertsGroup;
    [SerializeField] private AudioMixerGroup ambienceGroup;

    private AudioSource[] pool;
    private int cursor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"AudioVoice_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            pool[i] = src;
        }
    }

    public void PlayOneShot3D(AudioClip clip, Vector3 worldPos, AudioBusType bus,
        float volume = 1f, float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        if (clip == null) return;

        var src = NextVoice();
        src.outputAudioMixerGroup = ResolveBus(bus);
        src.transform.position = worldPos;
        src.pitch = Random.Range(minPitch, maxPitch);
        src.volume = volume;
        src.clip = clip;
        src.Play();
    }

    private AudioSource NextVoice()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            int idx = (cursor + i) % pool.Length;
            if (!pool[idx].isPlaying)
            {
                cursor = (idx + 1) % pool.Length;
                return pool[idx];
            }
        }

        // Voice stealing fallback (oldest by cursor)
        var stolen = pool[cursor];
        cursor = (cursor + 1) % pool.Length;
        return stolen;
    }

    private AudioMixerGroup ResolveBus(AudioBusType bus)
    {
        return bus switch
        {
            AudioBusType.Weapons => weaponsGroup,
            AudioBusType.Impacts => impactsGroup,
            AudioBusType.UIAlerts => uiAlertsGroup,
            AudioBusType.Ambience => ambienceGroup,
            _ => impactsGroup
        };
    }
}
```

## Snippet B: Low-health cadence + ambience duck control

```csharp
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public sealed class LowHealthAlertController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private AudioSource alertSource;
    [SerializeField] private AudioMixer mixer;

    [Header("Thresholds")]
    [SerializeField, Range(0.01f, 1f)] private float lowHealthThreshold = 0.30f;

    [Header("Pulse Timing")]
    [SerializeField] private float slowPulseSeconds = 1.2f; // near 30% HP
    [SerializeField] private float fastPulseSeconds = 0.35f; // near 0% HP

    [Header("Mixer Params")]
    [SerializeField] private string ambienceDuckParam = "AmbienceDuck_dB"; // exposed param
    [SerializeField] private float duckDb = -10f;
    [SerializeField] private float releaseDb = 0f;
    [SerializeField] private float duckLerpSpeed = 8f;

    private Coroutine pulseRoutine;
    private float targetDuck;

    private void Update()
    {
        float hp01 = playerHealth.CurrentHp / playerHealth.MaxHp;
        bool low = hp01 <= lowHealthThreshold;

        if (low && pulseRoutine == null)
        {
            pulseRoutine = StartCoroutine(PulseLoop());
        }
        else if (!low && pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
            targetDuck = releaseDb;
        }

        if (low)
        {
            targetDuck = duckDb; // duck ambience only while alert is active
        }

        mixer.GetFloat(ambienceDuckParam, out float current);
        float next = Mathf.Lerp(current, targetDuck, Time.unscaledDeltaTime * duckLerpSpeed);
        mixer.SetFloat(ambienceDuckParam, next);
    }

    private IEnumerator PulseLoop()
    {
        while (true)
        {
            float hp01 = playerHealth.CurrentHp / playerHealth.MaxHp;
            float t = Mathf.InverseLerp(lowHealthThreshold, 0f, hp01);
            float interval = Mathf.Lerp(slowPulseSeconds, fastPulseSeconds, t);

            // Optional: map pitch to 700-1100 Hz perceptual urgency using semitone-like scaling.
            alertSource.pitch = Mathf.Lerp(1.0f, 1.25f, t);
            alertSource.Play();

            yield return new WaitForSecondsRealtime(interval);
        }
    }
}
```

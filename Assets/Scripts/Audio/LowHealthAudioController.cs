using UnityEngine;
using UnityEngine.Audio;

public sealed class LowHealthAudioController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Entity playerEntity;

    [Header("Alert Pulse")]
    [SerializeField] private AudioSource pulseSource;
    [SerializeField] private AudioEventData lowHealthEventData;
    [SerializeField, Range(0f, 1f)] private float lowHealthThreshold = 0.30f;
    [SerializeField] private float minPulseInterval = 0.30f;
    [SerializeField] private float maxPulseInterval = 1.20f;

    [Header("Ambience Ducking")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string ambienceVolumeParam = "AmbienceVolume_dB";
    [SerializeField] private float normalAmbienceDb = 0f;
    [SerializeField] private float lowDuckMinDb = -8f;
    [SerializeField] private float lowDuckMaxDb = -14f;
    [SerializeField] private float duckLerpSpeed = 8f;

    private float pulseTimer;
    private float targetAmbienceDb;

    private void OnEnable()
    {
        pulseTimer = 0f;
        targetAmbienceDb = normalAmbienceDb;
        if (mixer != null)
        {
            mixer.SetFloat(ambienceVolumeParam, normalAmbienceDb);
        }

        if (lowHealthEventData != null && lowHealthEventData.EventType != GameAudioEventType.LowHealth)
        {
            Debug.LogWarning($"{nameof(LowHealthAudioController)} expects a LowHealth AudioEventData on {name}.", this);
        }
    }

    private void Update()
    {
        if (playerEntity == null || playerEntity.MaxHp <= 0f)
        {
            return;
        }

        float hp01 = Mathf.Clamp01(playerEntity.Hp / playerEntity.MaxHp);
        bool isLowHealth = hp01 <= lowHealthThreshold;

        if (isLowHealth)
        {
            float severity = Mathf.InverseLerp(lowHealthThreshold, 0f, hp01);
            float interval = Mathf.Lerp(maxPulseInterval, minPulseInterval, severity);

            pulseTimer -= Time.unscaledDeltaTime;
            if (pulseTimer <= 0f)
            {
                pulseTimer = interval;
                if (AudioManager.Instance != null && lowHealthEventData != null)
                {
                    AudioManager.Instance.PlaySound(lowHealthEventData, transform.position);
                }
                else if (pulseSource != null)
                {
                    pulseSource.pitch = Mathf.Lerp(1f, 1.2f, severity);
                    pulseSource.Play();
                }
            }

            targetAmbienceDb = Mathf.Lerp(lowDuckMinDb, lowDuckMaxDb, severity);
        }
        else
        {
            pulseTimer = 0f;
            targetAmbienceDb = normalAmbienceDb;
        }

        if (mixer != null)
        {
            mixer.GetFloat(ambienceVolumeParam, out float currentDb);
            float nextDb = Mathf.Lerp(currentDb, targetAmbienceDb, Time.unscaledDeltaTime * duckLerpSpeed);
            mixer.SetFloat(ambienceVolumeParam, nextDb);
        }
    }
}

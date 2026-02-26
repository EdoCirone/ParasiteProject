using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAudioFeedback : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio")]
    [SerializeField] private AudioEventData onHoverAudioEventData;
    [SerializeField] private AudioEventData onClickAudioEventData;
    [SerializeField, Min(0f)] private float hoverMinInterval = 0.05f;

    private float nextHoverAudioTime;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Time.unscaledTime < nextHoverAudioTime) return;

        nextHoverAudioTime = Time.unscaledTime + hoverMinInterval;
        TryPlayAudio(onHoverAudioEventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TryPlayAudio(onClickAudioEventData);
    }

    private void TryPlayAudio(AudioEventData eventData)
    {
        if (!eventData) return;

        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager) return;

        audioManager.PlaySound(eventData, transform.position);
    }
}

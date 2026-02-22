using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    [Header("Event Assets")]
    public AudioEventData shotData;
    public AudioEventData hitData;
    public AudioEventData deathData;
    public AudioEventData jumpData;

    [Header("Health Simulation")]
    public LowHealthAudioController healthController;
    [Range(0, 100)] public float simulatedHP = 100f;

    void Update()
    {
        // Test Eventi Singoli
        if (Input.GetKeyDown(KeyCode.Alpha1)) AudioManager.Instance.PlaySound(shotData, transform.position);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AudioManager.Instance.PlaySound(hitData, transform.position);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AudioManager.Instance.PlaySound(deathData, transform.position);
        if (Input.GetKeyDown(KeyCode.Alpha4)) AudioManager.Instance.PlaySound(jumpData, transform.position);

        // Test Salute (Frecce Su/Giù)
        if (Input.GetKey(KeyCode.UpArrow)) simulatedHP = Mathf.Min(100, simulatedHP + 0.5f);
        if (Input.GetKey(KeyCode.DownArrow)) simulatedHP = Mathf.Max(0, simulatedHP - 0.5f);

        // Aggiorna forzatamente il controller se presente
        if (healthController != null)
        {
            // Nota: Assicurati che il tuo HealthController abbia un modo 
            // per ricevere questo valore (es. una variabile pubblica o un metodo)
            healthController.SetHealthForDebug(simulatedHP / 100f);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Player_Controller : Entity
{
    [SerializeField] private float possessRadius = 3f;
    [SerializeField] private float percentHpForLoss = 70f;
    [SerializeField] private float velocityLossHp = 4f;
    [SerializeField] private float hpLoseOverTime = 10f;

    [Header("Audio")]
    [SerializeField] private AudioEventData parasitePossessionAudioEventData;
    [SerializeField] private AudioEventData onDashAudioEventData;

    private bool _isDeath;
    private Coroutine _hpRoutine;
    private UnityEvent _playerDeath = new UnityEvent();
    private InGameMenuManager _inGameMenuManager;


    public override void Awake()
    {
        base.Awake();
        _inGameMenuManager = FindFirstObjectByType<InGameMenuManager>();

        if (_inGameMenuManager == null)
        {
            Debug.LogError("Player_Controller: InGameMenuManager not found in the scene. Please ensure there is an InGameMenuManager in the scene.");
            return;
        }

        _playerDeath.AddListener(_inGameMenuManager.OnPlayerDeath);
    }

    private void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlayAudio(onDashAudioEventData, transform.position);
            TakeControllBody();
        }
    }

    private void TakeControllBody()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, possessRadius,layerEnemy);

        Debug.Log($"Player_Controller: Found {hits.Length} colliders within possess radius.");
        if (hits.Length == 0) return;

        float minDist = Mathf.Infinity;
        EnemyController closest = null;

        foreach (Collider2D hit in hits)
        {
            EnemyController ec = hit.GetComponent<EnemyController>();

            Debug.Log($"Player_Controller: Checking collider {hit.name} for EnemyController component. is deaht {ec.IsDeath}");
            if (!ec || !ec.IsDeath) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = ec;
            }
        }

        if (!closest) return;
        TryPlayAudio(parasitePossessionAudioEventData, closest.transform.position);
        entity = closest.Entity_SO;
        SetEntity();
    }

    public override void SetEntity()
    {
        base.SetEntity();
        _isDeath = false; //Resetto la morte per sicurezza
        if (_hpRoutine != null) StopCoroutine(_hpRoutine); //Evito doppie coroutine
        _hpRoutine = StartCoroutine(DecreseHpRoutine());
    }

    public override void Damage(float damage)
    {
        if (_isDeath) return;

        base.Damage(damage);

        if (hp <= 0)
        {
            hp = 0;
            _isDeath = true;

            if (_hpRoutine != null)
            {
                StopCoroutine(_hpRoutine);
                _hpRoutine = null;
            }

            PlayDeathAudio();
            _playerDeath?.Invoke();

            if (isPlayer) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }

    }


    private IEnumerator DecreseHpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(velocityLossHp);
            float hpPercent = hp / maxHp;
            if (hpPercent < percentHpForLoss / 100f)
            {
                Damage(hpLoseOverTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, possessRadius);
    }

}

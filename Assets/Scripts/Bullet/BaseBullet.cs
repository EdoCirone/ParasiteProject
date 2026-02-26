using System.Collections;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField, Min(0f)] private float onHitMinInterval = 0.05f;
    [SerializeField] protected AudioEventData onHitAudioEventData;

    protected Rigidbody2D rb;
    protected Vector2 direction;
    protected float speedAdd;
    protected float damage;
   
    protected float nextOnHitAudioTime;

    protected Pool_Obj pool;

    public LayerMask LayerEnemey;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(LifeTimeRoutine());
    }

    public virtual void OnDisable()
    {
        StopAllCoroutines();
        rb.angularVelocity = 0f;
        direction = Vector2.zero;
    }

    public virtual void FixedUpdate()
    {
        rb.linearVelocity = direction.normalized * (speed + speedAdd);
    }

    public void SetDirectionAndSpeed(Vector2 dir,float speed,float damage, AudioEventData onHitAudioEventData = null)
    {
        direction = dir;
        speedAdd = speed;
        this.damage = damage;
        this.onHitAudioEventData = onHitAudioEventData;
    }

    public virtual IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!pool) pool = GetComponentInChildren<Pool_Obj>();
        pool.ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((LayerEnemey.value & (1 << other.gameObject.layer)) != 0)
        {
            Entity e = other.GetComponentInChildren<Entity>();
            if (e)
            {
                e.Damage(damage);

                if (Time.time >= nextOnHitAudioTime)
                {
                    nextOnHitAudioTime = Time.time + onHitMinInterval;
                    TryPlayAudio(onHitAudioEventData, other.transform.position);
                }
            }

            if (!pool) pool = GetComponentInChildren<Pool_Obj>();
            pool.ReturnToPool();
        }
    }

    protected void TryPlayAudio(AudioEventData eventData, Vector3 position)
    {
        if (!eventData) return;

        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager) return;

        audioManager.PlaySound(eventData, position);
    }
}

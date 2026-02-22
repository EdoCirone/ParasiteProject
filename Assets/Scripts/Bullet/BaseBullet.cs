using System.Collections;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float lifeTime = 5f;

    protected Rigidbody2D rb;
    protected Vector2 direction;
    protected float speedAdd;
    protected float damage;

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

    public void SetDirectionAndSpeed(Vector2 dir,float speed,float damage)
    {
        direction = dir;
        speedAdd = speed;
        this.damage = damage;
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
            if (e) e.Damage(damage);

            if (!pool) pool = GetComponentInChildren<Pool_Obj>();
            pool.ReturnToPool();
        }
    }
}

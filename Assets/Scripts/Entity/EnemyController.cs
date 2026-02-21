using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : Entity
{
    [SerializeField] protected float timerForDeath = 3f;

    protected Pool_Obj pool;
    protected Transform player;

    public override void Awake()
    {
        base.Awake();

        player = GameObject.FindGameObjectWithTag("Player").transform;     
    }

    public virtual void Update()
    {
        if(!player || isDeath) return;

        Vector2 direction = player.position - transform.position;
        X = direction.x;
        Y = direction.y;
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (hp < 0)
        {
            hp = 0;
            isDeath = true;
            rb.linearVelocity = Vector3.zero;

            StartCoroutine(DyingRoutine());
        }
    }

    private IEnumerator DyingRoutine()
    {
        yield return new WaitForSeconds(timerForDeath);
        if(!pool) pool = GetComponentInChildren<Pool_Obj>();

        pool.ReturnToPool();
    }
}

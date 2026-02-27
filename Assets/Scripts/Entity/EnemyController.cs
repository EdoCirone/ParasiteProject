using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : Entity
{
    [SerializeField] protected float timerForDeath = 3f;

    protected Pool_Obj pool;
    protected PointManager pointManager;
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
        if(isDeath) return; 

        base.Damage(damage);

        if (hp > 0) return;

        hp = 0;
        isDeath = true;
        PlayDeathAudio();
        rb.linearVelocity = Vector3.zero;

        if (!pointManager) pointManager = PointManager.Instance;
        if (pointManager) pointManager.AddPoints(entity.Score);

        StartCoroutine(DyingRoutine());
    }

    private IEnumerator DyingRoutine()
    {
        yield return new WaitForSeconds(timerForDeath);
        if(!pool) pool = GetComponentInChildren<Pool_Obj>();

        if (pool) pool.ReturnToPool();
        else Destroy(gameObject);
    }
}

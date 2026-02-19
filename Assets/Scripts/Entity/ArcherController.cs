using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ArcherController : EnemyController
{
    [SerializeField] private Vector2 radius = new Vector2(3f, 6f);
    [SerializeField] private Vector2 changeDir = new Vector2(5f, 10f);

    private float currentRadius;
    private int directionRotation = 1;

    public override void OnEnable()
    {
        base.OnEnable();

        PickNewOrbit();
        StartCoroutine(ChangeRoutine());
    }

    public override void FixedUpdate()
    {
        if (!player || isDeath) return;

        Vector2 toEnemy = transform.position - player.position;
        float distToEnemy = toEnemy.magnitude;
        Vector2 radialDir = toEnemy.normalized;

        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x) * directionRotation;

        float radius = distToEnemy - currentRadius;
        Vector2 correction = -radialDir * radius * 2f;

        Vector2 finalDir = (tangentDir + correction).normalized;
        Moving(finalDir.x, finalDir.y);;
    }


    public override void Update()
    {
    }

    public override void Moving(float x, float y)
    {
        Vector2 direction = new Vector2(x, y).normalized;

        float speed = isPlayer ? entity.Speed * entity.BoostPlayerSpeed : entity.Speed / 2;
        rb.linearVelocity = direction.normalized * speed;

        if (direction.sqrMagnitude > 0.01f)
        {
            facingDir = (player.position - transform.position).normalized;
            RotateVisual(facingDir);
        }
    }

    IEnumerator ChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(changeDir.x,changeDir.y));
            PickNewOrbit();
        }
    }

    void PickNewOrbit()
    {
        currentRadius = Random.Range(radius.x, radius.y);
        directionRotation = Random.value > 0.5f ? 1 : -1;

        Vector2 dir = transform.position - player.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player.position, currentRadius);
    }
}

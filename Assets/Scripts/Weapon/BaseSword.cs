using System.Collections;
using UnityEngine;

public class BaseSword : MonoBehaviour
{
    public float upAngle = 70f;
    public float downAngle = -120f;

    public float windUpTime = 0.15f;
    public float swingTime = 0.08f;
    public float returnTime = 0.12f;


    private LayerMask layerEnemey;
    private float damage;

    public void Attack(Transform attackPoint, Entity entity)
    {
        StartCoroutine(AttackRoutine(attackPoint, entity));

        layerEnemey = entity.LayerEnemy;
        damage = entity.GetDamage();
    }

    private IEnumerator AttackRoutine(Transform attackPoint, Entity entity)
    {
        yield return null;
        transform.localPosition = Vector3.zero;
        Transform weapon = transform;
        Quaternion startRot = weapon.localRotation;

        Quaternion upRot = Quaternion.Euler(0, 0, upAngle);
        Quaternion downRot = Quaternion.Euler(0, 0, downAngle );

        yield return Rotate(weapon, startRot, upRot, windUpTime);
        yield return Rotate(weapon, upRot, downRot, swingTime);
        yield return Rotate(weapon, downRot, startRot, returnTime);

        entity.StartAttack();

        Destroy(gameObject);
    }

    private IEnumerator Rotate(Transform t, Quaternion a, Quaternion b, float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            t.localRotation = Quaternion.Slerp(a, b, elapsed / time);
            yield return null;
        }

        t.localRotation = b;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((layerEnemey.value & (1 << other.gameObject.layer)) != 0)
        {
            Entity e = other.GetComponentInChildren<Entity>();
            if (e) e.Damage(damage);
        }
    }
}

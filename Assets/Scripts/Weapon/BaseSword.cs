using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackSequenceMelee
{
    public Vector3 AttackPosition;
    public Vector3 AttackRotation;
    public float AttackDuration;
}

public class BaseSword : MonoBehaviour
{
    [SerializeField] private List<AttackSequenceMelee> attackSequenceMelees = new List<AttackSequenceMelee>();

    [Header("Debug")]
    public LayerMask layerEnemey;
    public float damage;

    private Pool_Obj pool;
    private float flipX;
    private float flipY;

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
  
        Quaternion startRot = transform.localRotation;
        flipX = Mathf.Sign(entity.FacingDir.x == 0 ? 1 : entity.FacingDir.x);
        flipY = Mathf.Sign(entity.FacingDir.y == 0 ? 1 : entity.FacingDir.y);

        for (int i = 0; i < attackSequenceMelees.Count; i++)
        {
            AttackSequenceMelee currentAttack = attackSequenceMelees[i];

            RotationWep(currentAttack,startRot);
            LocationWep(currentAttack);

            yield return new WaitForSeconds(currentAttack.AttackDuration);
        }
        entity.StartAttack();

        if(!pool) pool = GetComponentInChildren<Pool_Obj>();

        if(pool) pool.ReturnToPool();
        else Destroy(gameObject);
    }

    private void LocationWep(AttackSequenceMelee attack)
    {
        float distance = attack.AttackPosition.x;

        Vector3 forwardDir = transform.right;
        Vector3 adjustedPosition = forwardDir * distance;

        transform.DOLocalMove(adjustedPosition, attack.AttackDuration);
    }

    private void RotationWep(AttackSequenceMelee attack,Quaternion startRot)
    {
        Vector3 adjustedRotation = attack.AttackRotation;
        adjustedRotation.z *= flipX;

        Quaternion wepRotation = startRot * Quaternion.Euler(adjustedRotation);
        transform.DOLocalRotateQuaternion(wepRotation, attack.AttackDuration);
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

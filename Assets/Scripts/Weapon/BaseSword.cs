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
  
        Quaternion startRot = transform.localRotation;
        float flip = Mathf.Sign(entity.FacingDir.x == 0 ? 1 : entity.FacingDir.x);

        for (int i = 0; i < attackSequenceMelees.Count; i++)
        {
            AttackSequenceMelee currentAttack = attackSequenceMelees[i];

            LocationWep(currentAttack,flip);
            RotationWep(currentAttack,flip,startRot);

            yield return new WaitForSeconds(currentAttack.AttackDuration);
        }
        entity.StartAttack();
        Destroy(gameObject);
    }

    private void LocationWep(AttackSequenceMelee attack, float flip)
    {
        Vector3 adjustedPosition = attack.AttackPosition;
        adjustedPosition.x *= flip;
        adjustedPosition.y *= flip;
        adjustedPosition.z *= flip;

        transform.DOLocalMove(adjustedPosition, attack.AttackDuration);
    }

    private void RotationWep(AttackSequenceMelee attack,float flip,Quaternion startRot)
    {
        Vector3 adjustedRotation = attack.AttackRotation;
        adjustedRotation.z *= flip;

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

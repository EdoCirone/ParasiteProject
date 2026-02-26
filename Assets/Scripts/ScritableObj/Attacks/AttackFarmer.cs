using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Attacks", menuName = "AttackFarmer")]
public class AttackFarmer : AttackBase_SO
{
    [SerializeField] private BaseSword baseSword;
    [Header("Audio")]
    [SerializeField] private AudioEventData swingAudioEventData;

    public override void Attack(Transform attackPoint, Entity entity)
    {   
        BaseSword sword = Instantiate(baseSword, attackPoint.position, Quaternion.identity,attackPoint);

        Vector2 dir = entity.FacingDir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float weaponOffset = 180f;
        sword.transform.localRotation = Quaternion.Euler(0f, 0f, angle +weaponOffset);

        sword.transform.localPosition = Vector3.zero;

        if (swingAudioEventData && AudioManager.Instance)
            AudioManager.Instance.PlaySound(swingAudioEventData, attackPoint.position);

        sword.Attack(attackPoint,entity);
    }
}

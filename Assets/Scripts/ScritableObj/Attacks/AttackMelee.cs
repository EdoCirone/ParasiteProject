using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Attacks", menuName = "AttackMelee")]
public class AttackMelee : AttackBase_SO
{
    public override void Attack(Transform attackPoint, Entity entity)
    {
        GameObject obj = wepon.SpawnObj(attackPoint.position, Quaternion.identity);
        obj.transform.SetParent(attackPoint);

        BaseSword sword = obj.GetComponent<BaseSword>();

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = entity.IsPlayer ? (mouseWorldPos - (Vector2)attackPoint.position).normalized : entity.FacingDir;

        if (!canUseY) dir = new Vector2(Mathf.Sign(dir.x == 0 ? 1 : dir.x), 0);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float weaponOffset = 180f;
        sword.transform.localRotation = Quaternion.Euler(0f, 0f, angle + weaponOffset);

        sword.transform.localPosition = Vector3.zero;
        sword.Attack(attackPoint,entity);
    }
}

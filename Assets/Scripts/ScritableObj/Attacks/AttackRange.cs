using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "Attacks", menuName = "AttackRange")]
public class AttackRange : AttackBase_SO
{
    [SerializeField] private BaseBullet baseBullet;
    [SerializeField] private int attackOnSequence = 1;
    [SerializeField] private float timeEachAttack = 0.25f;
    [SerializeField] private bool canUseY;

    public override void Attack(Transform attackPoint, Entity entity)
    {
        Rigidbody2D rbEntity = entity.GetComponent<Rigidbody2D>();
        entity.StartCoroutine(AttackOnSequence(attackPoint,entity,rbEntity));     
    }  
    
    private void Shooting(Transform attackPoint, Entity entity, Rigidbody2D rbEntity)
    {
        Vector2 dir = entity.FacingDir;
        if (!canUseY) dir = new Vector2(Mathf.Sign(dir.x == 0 ? 1 : dir.x), 0);

        dir.Normalize();

        BaseBullet bullet = Instantiate(baseBullet, attackPoint.position, Quaternion.identity);
        bullet.SetDirectionAndSpeed(dir, rbEntity.linearVelocity.magnitude, entity.GetDamage());
        bullet.LayerEnemey = entity.LayerEnemy;

        bullet.transform.up = dir;
    }

    private IEnumerator AttackOnSequence(Transform attackPoint, Entity entity, Rigidbody2D rbEntity)
    {
        for(int i = 0; i < attackOnSequence; i++)
        {
            Shooting(attackPoint, entity, rbEntity);
            yield return new WaitForSeconds(timeEachAttack);
        }
        entity.StartAttack();
    }
}

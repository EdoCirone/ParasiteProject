using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "Attacks", menuName = "AttackRange")]
public class AttackRange : AttackBase_SO
{
    [SerializeField] private int attackOnSequence = 1;
    [SerializeField] private float timeEachAttack = 0.25f;
    [Header("Audio")]
    [SerializeField] private AudioEventData onAttackAudioEventData;
    [SerializeField] private AudioEventData onHitAudioEventData;
    [SerializeField] private AudioEventData onReloadAudioEventData;

    public override void Attack(Transform attackPoint, Entity entity)
    {
        Rigidbody2D rbEntity = entity.GetComponent<Rigidbody2D>();
        entity.StartCoroutine(AttackOnSequence(attackPoint,entity,rbEntity));     
    }  
    
    private void Shooting(Transform attackPoint, Entity entity, Rigidbody2D rbEntity)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = entity.IsPlayer ? (mouseWorldPos - (Vector2)attackPoint.position).normalized : entity.FacingDir;
        if (!canUseY) dir = new Vector2(Mathf.Sign(dir.x == 0 ? 1 : dir.x), 0);
        dir.Normalize();

        GameObject bulletObj = wepon.SpawnObj(attackPoint.position, Quaternion.identity);

        BaseBullet bullet = bulletObj.GetComponent<BaseBullet>();
        if (!bullet) return;

        TryPlayAudio(onAttackAudioEventData, attackPoint.position);
        bullet.SetDirectionAndSpeed(dir, rbEntity.linearVelocity.magnitude, entity.GetDamage(), onHitAudioEventData);
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
        TryPlayAudio(onReloadAudioEventData, attackPoint.position);
        entity.StartAttack();
    }

    private void TryPlayAudio(AudioEventData eventData, Vector3 position)
    {
        if (!eventData) return;

        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager) return;

        audioManager.PlaySound(eventData, position);
    }
}

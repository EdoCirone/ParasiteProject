using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Controller : Entity
{
    [SerializeField] private float possessRadius = 3f;
    [SerializeField] private float percentHpForLoss = 70f;
    [SerializeField] private float velocityLossHp = 4f;
    [SerializeField] private float hpLoseOverTime = 10f;

    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) TakeControllBody();
    }

    private void TakeControllBody()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, possessRadius, layerEnemy);

        if (hits.Length == 0) return;

        float minDist = Mathf.Infinity;
        EnemyController closest = null;

        foreach (Collider2D hit in hits)
        {
            EnemyController ec = hit.GetComponent<EnemyController>();
            if (!ec || !ec.IsDeath) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = ec;
            }
        }

        if (!closest) return;
        entity = closest.Entity_SO;
        SetEntity();
    }

    public override void SetEntity()
    {
        base.SetEntity();
        StartCoroutine(DecreseHpRoutine());
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (hp < 0)
        {
            hp = 0;

            if (isPlayer) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator DecreseHpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(velocityLossHp);
            float hpPercent = hp / maxHp;
            if (hpPercent < percentHpForLoss / 100f)
            {
                Damage(hpLoseOverTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, possessRadius);
    }

}

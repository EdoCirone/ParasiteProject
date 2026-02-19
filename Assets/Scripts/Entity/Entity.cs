using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Entity_SO entity;
    [SerializeField] protected Transform visual;
    [SerializeField] protected bool isPlayer;
    [SerializeField] protected LayerMask layerEnemy;

    protected Rigidbody2D rb;
    protected float maxHp;
    protected float hp;
    protected bool isDeath;
    protected Vector2 facingDir = Vector2.right;

    public Vector2 FacingDir => facingDir;

    public float X;
    public float Y;

    public bool IsDeath => isDeath;
    public LayerMask LayerEnemy => layerEnemy;

    public Entity_SO Entity_SO => entity;

    public float Hp => hp;
    public float MaxHp => maxHp;    

    public virtual void OnEnable()
    {
        SetEntity();
        isDeath = false;
    }

    public virtual void OnDisable()
    {
        StopAllCoroutines();
        isDeath = false;
    }

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    public virtual void FixedUpdate()
    {
        if(IsDeath)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        Moving(X, Y);
    }

    public virtual void Moving(float x,float y)
    {
        Vector2 direction = new Vector2(x,y).normalized;

        float speed = isPlayer ? entity.Speed * entity.BoostPlayerSpeed : entity.Speed / 2;
        rb.linearVelocity = direction.normalized * speed;

        if (direction.sqrMagnitude > 0.01f)
        {
            facingDir = direction.normalized;
            RotateVisual(facingDir);
        }
    }

    public virtual void RotateVisual(Vector2 dir)
    {
        if (dir.x == 0) return;

        Vector3 scale = visual.localScale;
        scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
        visual.localScale = scale;
    }



    public virtual void SetEntity()
    {
        StopAllCoroutines();
        foreach(Transform t in visual) Destroy(t.gameObject);
        Instantiate(entity.Visual, visual);

        maxHp = entity.MaxHp;
        hp = maxHp;

        StartCoroutine(AttackRoutine());
    }

    public void StartAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        float speedAttack = isPlayer ? entity.VelocityAttack * entity.BoostPlayerVelocityAttack : entity.VelocityAttack;
        yield return new WaitForSeconds(speedAttack);
        if(isDeath) yield break;
        entity.Attack.Attack(transform, this);
    }

    public float GetDamage() => isPlayer? entity.Damage * entity.BoostPlayerDamage : entity.Damage;

    public virtual void Damage(float damage)
    {
        hp -= damage;

        //Danno Visivo
        //Rumore?
    }

}

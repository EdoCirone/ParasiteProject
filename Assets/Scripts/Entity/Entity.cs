using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Entity_SO entity;
    [SerializeField] protected Transform visual;
    [SerializeField] protected bool isPlayer;
    [SerializeField] protected LayerMask myLayer;
    [SerializeField] protected LayerMask layerEnemy;
    [Header("Audio")]
    [SerializeField] protected AudioEventData damageAudioEventData;
    [SerializeField] protected AudioEventData deathAudioEventData;
    [SerializeField, Min(0f)] private float damageAudioMinInterval = 0.05f;

    protected Rigidbody2D rb;
    protected LifeSystem lifeSystem;
    protected Animator animator;
    protected float maxHp;
    [SerializeField]protected float hp;
    protected bool isDeath;
    protected Vector2 facingDir = Vector2.right;
    private float nextDamageAudioTime;

    public Vector2 FacingDir => facingDir;

    public float X;
    public float Y;

    public bool IsDeath => isDeath;
    public LayerMask LayerEnemy => layerEnemy;

    public Entity_SO Entity_SO => entity;

    public bool IsPlayer => isPlayer;   

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

        if (animator)
        {
            float magnitude = direction.magnitude;
            bool isMoving = magnitude > 0.01f;

            animator.SetBool("isMoving", isMoving);

            animator.SetFloat("x", x);
            animator.SetFloat("y", y);
        }
        else
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                facingDir = direction.normalized;
                RotateVisual(facingDir);
            }
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
        StartCoroutine(SetUpRoutine());
    }

    private IEnumerator SetUpRoutine()
    {
        foreach (Transform t in visual) Destroy(t.gameObject);
        Instantiate(entity.Visual, visual);

        for (int i = 0; i < 5; i++) yield return null;
        lifeSystem = GetComponentInChildren<LifeSystem>();
        lifeSystem.SetUp(this, myLayer);

        maxHp = entity.MaxHp;
        hp = maxHp;

        animator = GetComponentInChildren<Animator>();

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

        if (Time.time >= nextDamageAudioTime)
        {
            nextDamageAudioTime = Time.time + damageAudioMinInterval;
            TryPlayAudio(damageAudioEventData, transform.position);
        }

        //Danno Visivo
        //Rumore?
    }

    protected void PlayDeathAudio()
    {
        TryPlayAudio(deathAudioEventData, transform.position);
    }

    protected void TryPlayAudio(AudioEventData eventData, Vector3 position)
    {
        if (!eventData) return;

        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager) return;

        audioManager.PlaySound(eventData, position);
    }

}

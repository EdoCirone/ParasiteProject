using UnityEngine;

[CreateAssetMenu(fileName = "Entity_SO")]
public class Entity_SO : ScriptableObject
{
    public float MaxHp = 100f;
    public float Speed = 1f;
    public float BoostPlayerSpeed = 1.25f;

    public float Damage = 1f;
    public float BoostPlayerDamage = 1.25f;

    public float VelocityAttack = 1f;
    public float BoostPlayerVelocityAttack = 1.25f;

    public int Score = 1;

    public GameObject Visual;
    public AttackBase_SO Attack;
}

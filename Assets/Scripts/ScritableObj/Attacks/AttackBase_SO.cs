using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attacks")]
public class AttackBase_SO : ScriptableObject
{
    [SerializeField] protected bool canUseY;


    public virtual void Attack(Transform attackPoint, Entity entity) { }
}

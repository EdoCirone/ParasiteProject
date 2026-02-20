using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attacks")]
public class AttackBase_SO : ScriptableObject
{
    public virtual void Attack(Transform attackPoint, Entity entity) { }
}

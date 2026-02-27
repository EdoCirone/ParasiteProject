using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    private Entity entity;
    private LayerMask myLayer;

    public LayerMask ObjLayer => myLayer;

    public void SetUp(Entity entity,LayerMask validLayerMask)
    {
        this.entity = entity;
        myLayer = validLayerMask;
    }

    public void Damage(float damage, LayerMask attackerLayer)
    {
        if (myLayer != attackerLayer) return;
        if (entity) entity.Damage(damage);
    }
}

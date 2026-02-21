using UnityEngine;

[CreateAssetMenu(fileName = "PoolObject", menuName = "Pooling/Pool Object")]
public class PoolObject_SO : ScriptableObject
{
    [Header("Pooling Settings")]
    public GameObject PreFab;
    public string ID;

    public int StartSize = 10;

    [Header("Lifecycle")]
    public float LifeTime = 5;
    public bool IsIndetermitate = false;

    public virtual GameObject SpawnObj(Vector3 position, Quaternion rotation)
    {
        if (PollingEnemy.Instance)
        {
            GameObject obj = PollingEnemy.Instance.GetObjFromPool(this, position, rotation);
            return obj;
        }
        else
        {
            Debug.LogError("No PollingEnemy in Scene");
            return null;
        }
    }
}
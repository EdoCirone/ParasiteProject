using System.Collections.Generic;
using UnityEngine;

public class PollingEnemy : MonoBehaviour
{
    [Header("Pool Config")]
    [SerializeField] private List<PoolObject_SO> poolObjectsList = new List<PoolObject_SO>();

    private Dictionary<string, GameObject> spawnDictionaryParent = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<Pool_Obj>> poolDictionary = new Dictionary<string, Queue<Pool_Obj>>();

    public static PollingEnemy Instance { get; private set; }

    public void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }

        Instance = GetComponent<PollingEnemy>();
        GeneratePool();
    }

    #region Create Pool
    private void GeneratePool()
    {
        if (poolObjectsList.Count <= 0) return;

        foreach (PoolObject_SO poolObj_SO in poolObjectsList) CreatePool(poolObj_SO);
    }

    private void CreatePool(PoolObject_SO poolObj_SO)
    {
        Queue<Pool_Obj> objectPool = new Queue<Pool_Obj>();

        if (!spawnDictionaryParent.TryGetValue(poolObj_SO.ID, out GameObject objParent))
        {
            objParent = new GameObject("ID " + poolObj_SO.ID);
            objParent.transform.parent = transform;
            spawnDictionaryParent.Add(poolObj_SO.ID, objParent);
        }

        for (int i = 0; i < poolObj_SO.StartSize; i++)
        {
            Pool_Obj poolObj = CreateNewObject(poolObj_SO, objParent.transform);
            objectPool.Enqueue(poolObj);
        }

        poolDictionary.Add(poolObj_SO.ID, objectPool);
    }

    private Pool_Obj CreateNewObject(PoolObject_SO poolObj_SO, Transform parent)
    {
        GameObject obj = Instantiate(poolObj_SO.PreFab, parent);
        obj.SetActive(false);

        Pool_Obj poolObj = obj.GetComponent<Pool_Obj>();
        if (poolObj == null) poolObj = obj.AddComponent<Pool_Obj>();

        return poolObj;
    }

    #endregion

    #region GetObjFromPool

    public GameObject GetObjFromPool(PoolObject_SO poolObj_SO, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolObj_SO.ID))
        {
            Debug.LogWarning($"[PollingEnemy] Pool '{poolObj_SO.ID}' non esisteva. Creazione al volo.");
            CreatePool(poolObj_SO);
        }

        Pool_Obj objToSpawn = null;
        Queue<Pool_Obj> pool = poolDictionary[poolObj_SO.ID];

        if (pool.Count > 0) objToSpawn = pool.Dequeue();
        else objToSpawn = CreateNewObject(poolObj_SO, spawnDictionaryParent[poolObj_SO.ID].transform);

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        objToSpawn.SetUp(poolObj_SO.LifeTime, poolObj_SO.ID, poolObj_SO.IsIndetermitate);
        objToSpawn.gameObject.SetActive(true);

        return objToSpawn.gameObject;
    }

    #endregion

    public void ReturnToPool(string id, Pool_Obj obj)
    {
        if (!poolDictionary.TryGetValue(id, out Queue<Pool_Obj> pool))
        {
            Debug.LogError("No ObjType Correct From Return Extra");
            Destroy(obj.gameObject);
            return;
        }

        obj.gameObject.SetActive(false);
        poolDictionary[id].Enqueue(obj);
    }
}

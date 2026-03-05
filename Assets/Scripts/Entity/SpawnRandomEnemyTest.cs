using System.Collections;
using UnityEngine;

public class SpawnRandomEnemyTest : MonoBehaviour
{
    [SerializeField] private PoolObject_SO[] enemys;
    [SerializeField] private float timer = 20f;

    [SerializeField] private float spawnRadius = 25f;
    [SerializeField] private float minDistanceFromPlayer = 15f;
    [SerializeField] private int maxSpawnAttempts = 5;
    [SerializeField] private LayerMask spawnLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private int minEnemySpawn = 1;
    [SerializeField] private int maxEnemySpawn = 3;

    private void Awake()
    {
        GameObject poolEnemy = new GameObject("PoolEnemy");
        poolEnemy.AddComponent<PollingEnemy>();
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return null;
        while (true)
        {
            int enmySpawn = Random.Range(1, minEnemySpawn);

            for (int i = 0; i < enmySpawn; i++)
            {
                TrySpawnEnemy();

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(timer);
        }
    }

    private void TrySpawnEnemy()
    {
        Debug.Log("Try Spawn Enemy");
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            if (Vector2.Distance(randomPos, transform.position) < minDistanceFromPlayer) continue;

            Collider2D hit = Physics2D.OverlapCircle(randomPos, 0.5f, spawnLayer);

            if(hit == null) Debug.Log("Hit nothing");
            else Debug.Log("Hit: " + hit?.name + " Layer " + hit.transform.gameObject.layer);

            if (hit != null)
            {
                GameObject obj = enemys[Random.Range(0, enemys.Length)].SpawnObj(randomPos, Quaternion.identity);
                return;
            }
        }
    }
}

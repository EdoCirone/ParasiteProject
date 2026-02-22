using System.Collections;
using UnityEngine;

public class SpawnRandomEnemyTest : MonoBehaviour
{
    [SerializeField] private PoolObject_SO[] enemys;
    [SerializeField] private float timer = 20f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int enmySpawn = Random.Range(1, 20);

            for (int i = 0; i < enmySpawn; i++)
            {
                enemys[Random.Range(0, enemys.Length)].SpawnObj(Random.insideUnitCircle * 15f, Quaternion.identity);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(timer);
        }
    }
}

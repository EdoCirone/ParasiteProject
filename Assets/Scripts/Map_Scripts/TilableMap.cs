using UnityEngine;
using System.Collections;

public class TilableMap : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private float _mapChunckSize;
    [SerializeField] private GameObject[] _mapChuncks;

    [Header("Organization")]
    [SerializeField] private Transform _mapParent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                GenerateChunck(x, y);
            }
        }
    }

    private void GenerateChunck(int x, int y)
    {
        if (_mapChuncks.Length == 0) { Debug.LogError("No map chuncks assigned!"); return; }
        ;

        int randomIndex = Random.Range(0, _mapChuncks.Length);
        GameObject prefab = _mapChuncks[randomIndex];

        Vector3 position = new Vector3(x * _mapChunckSize, y * _mapChunckSize, 0);
        Instantiate(prefab, position, Quaternion.identity, _mapParent);
    }

}

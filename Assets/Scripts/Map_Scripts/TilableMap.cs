using UnityEngine;
using System.Collections.Generic;

public class TilableMap : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private float _mapChunkSize;
    [SerializeField] private GameObject[] _mapChunks;
    [SerializeField] private int _mapSizeX;

    [Header("Organization")]
    [SerializeField] private Transform _mapParent;

    public int MapSizeX => _mapSizeX;
    public float MapChunkSize => _mapChunkSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_mapParent == null) { Debug.LogError("Map parent transform not assigned!"); return; }
        if (_mapChunks.Length == 0) { Debug.LogError("No map chuncks assigned!"); return; }

        GenerateMap();
    }

    private void GenerateMap()
    {

        List<Vector2> cells = new List<Vector2>();

        float half = (_mapSizeX - 1) * 0.5f;

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeX; y++)
            {
                cells.Add(new Vector2(x - half, y - half));
            }
        }

        Shuffle(cells);

        // Garantisce che ogni prefab venga utilizzato almeno una volta, se possibile
        int guaranteedCount = Mathf.Min(_mapChunks.Length, cells.Count);

        for (int i = 0; i < guaranteedCount; i++)
        {
            GenerateChunk(cells[i].x, cells[i].y, _mapChunks[i]);
        }

        //Genera i chunk rimanenti in modo casuale, se ci sono più celle che prefab disponibili

        for (int i = guaranteedCount; i < cells.Count; i++)
        {
            GameObject randomPrefab = _mapChunks[Random.Range(0, _mapChunks.Length)];
            GenerateChunk(cells[i].x, cells[i].y, randomPrefab);
        }

    }

    private void GenerateChunk(float x, float y, GameObject prefab)
    {
        if (prefab == null) { Debug.LogError("Prefab is null! Cannot generate chunck."); return; }

        Vector3 position = new Vector3(x * _mapChunkSize, y * _mapChunkSize, 0);
        Instantiate(prefab, position, Quaternion.identity, _mapParent);
    }


    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}

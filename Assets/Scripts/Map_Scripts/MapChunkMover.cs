using UnityEngine;
using UnityEngine.Tilemaps;

public class MapChunkMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform mapParent;
    [SerializeField] private TilableMap _tileMap;


    [Header("Settings")]

    [SerializeField] private float _distanceThreshold = 1.5f;


    private float CalculateDistanceThreshold => _tileMap.MapChunkSize * _distanceThreshold;
    private float RecycleOffset => _tileMap.MapChunkSize * _tileMap.MapSizeX;

    private void Awake()
    {
        if (_tileMap == null)
        {
            _tileMap = GetComponent<TilableMap>();

            if (_tileMap == null)
            {
                Debug.LogError("TilableMap component not found on MapChunkMover!");
                return;
            }
        }
    }

    void Update()
    {
        MoveChunk();
    }

    private void MoveChunk()
    {
        if (_player == null || mapParent == null) { Debug.LogError("Player or Map Parent not assigned!"); return; }

        for (int i = 0; i < mapParent.childCount; i++)
        {
            Transform chunk = mapParent.GetChild(i);

            Vector3 distance = _player.position - chunk.position;
            if (Mathf.Abs(distance.x) > CalculateDistanceThreshold)
            {
                chunk.position += Vector3.right * Mathf.Sign(distance.x) * RecycleOffset; ;
            }

            if (Mathf.Abs(distance.y) > CalculateDistanceThreshold)
            {
                chunk.position += Vector3.up * Mathf.Sign(distance.y) * RecycleOffset;
            }

        }
    }

}

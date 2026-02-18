using UnityEngine;

public class MapChunkMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform mapParent;

    [Header("Settings")]

    [SerializeField] private float _chunkSize = 25f;
    [SerializeField] private float _distanceThreshold = 1.5f;


    private float _calculateDistanceThreshold => _chunkSize * _distanceThreshold;

    void Update()
    {
        MoveChunk();
    }

    private void MoveChunk()
    {
        for (int i = 0; i < mapParent.childCount; i++)
        {
            Transform chunk = mapParent.GetChild(i);

            Vector3 distance = _player.position - chunk.position;
            if (Mathf.Abs(distance.x) > _calculateDistanceThreshold) 
            {
                chunk.position += Vector3.right * Mathf.Sign(distance.x) * _calculateDistanceThreshold * 2;
            } 
           
        }
    }

}

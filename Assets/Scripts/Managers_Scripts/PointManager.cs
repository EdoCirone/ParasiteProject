using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    private int currentPoints;

    public int CurrentPoints => currentPoints;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = GetComponent<PointManager>();
    }

    public void AddPoints(int points) => currentPoints += points;
}

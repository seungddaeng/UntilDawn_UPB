using UnityEngine;

public class RandomSpawnFromPoints : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenPoint = spawnPoints[randomIndex];

        transform.position = chosenPoint.position;
        transform.rotation = chosenPoint.rotation;
    }
}
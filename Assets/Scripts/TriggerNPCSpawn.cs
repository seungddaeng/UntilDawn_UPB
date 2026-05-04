using UnityEngine;

public class TriggerNPCSpawn : MonoBehaviour
{
    public Alexis alexis;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            alexis.transform.position = spawnPoint.position;
            alexis.AppearAndStartRoute();
            StoryTimeManager.Instance.TriggerDawn();

            Destroy(gameObject);
        }
    }
}
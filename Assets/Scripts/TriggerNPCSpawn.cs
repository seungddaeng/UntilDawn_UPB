using UnityEngine;

public class TriggerNPCSpawn : MonoBehaviour
{
    public AlexisScript alexis;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            alexis.transform.position = spawnPoint.position;
            alexis.UpdateVisibility();
            StoryTimeManager.Instance.TriggerDawn();

            Destroy(gameObject);
        }
    }
}
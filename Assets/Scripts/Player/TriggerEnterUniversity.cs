using UnityEngine;

public class TriggerEnterUniversity : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StoryTimeManager.Instance.TriggerNight();
        }
    }
}
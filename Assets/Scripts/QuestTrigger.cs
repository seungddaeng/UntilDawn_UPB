using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Marcelo
    }

    public TriggerType triggerType = TriggerType.Marcelo;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameQuestManager.Instance == null)
        {
            return;
        }

        if (triggerType == TriggerType.Marcelo)
        {
            GameQuestManager.Instance.OnMarceloReached();
        }
    }
}
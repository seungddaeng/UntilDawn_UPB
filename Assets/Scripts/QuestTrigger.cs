using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        EnterAlexisOffice,
        LibraryExitAfterReturningKey,
        CampusExitWin,
        EnterUniversity
    }

    public TriggerType triggerType;

    public bool onlyOnce = true;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (onlyOnce && alreadyTriggered)
        {
            return;
        }

        alreadyTriggered = true;

        if (triggerType == TriggerType.EnterUniversity)
        {
            if (StoryTimeManager.Instance != null)
            {
                StoryTimeManager.Instance.TriggerNight();
            }

            return;
        }

        if (GameQuestManager.Instance == null)
        {
            return;
        }

        switch (triggerType)
        {
            case TriggerType.EnterAlexisOffice:
                GameQuestManager.Instance.OnEnteredAlexisOffice();
                break;

            case TriggerType.LibraryExitAfterReturningKey:
                GameQuestManager.Instance.OnReturnedKeyAndExitedLibrary();
                break;

            case TriggerType.CampusExitWin:
                GameQuestManager.Instance.OnReachedCampusExit();
                break;
        }
    }
}
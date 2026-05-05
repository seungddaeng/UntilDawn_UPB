using UnityEngine;
using UnityEngine.InputSystem;

public class QuestTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        EnterAlexisOffice,
        LibraryExitAfterReturningKey,
        CampusExitWin,
        EnterUniversity,
        LibraryKeyReturn
    }

    public TriggerType triggerType;

    public bool onlyOnce = true;
    private bool alreadyTriggered = false;

    private bool playerInside = false;

    [Header("Interacción")]
    public string interactionMessage = "Presiona [E] para interactuar";

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (triggerType == TriggerType.LibraryKeyReturn)
        {
            HandleLibraryKeyReturn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        TryAutomaticTrigger();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        TryAutomaticTrigger();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        if (triggerType == TriggerType.LibraryKeyReturn && UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }

    private void TryAutomaticTrigger()
    {
        if (onlyOnce && alreadyTriggered)
        {
            return;
        }

        if (triggerType == TriggerType.LibraryKeyReturn)
        {
            return;
        }

        if (triggerType == TriggerType.EnterUniversity)
        {
            alreadyTriggered = true;

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
                if (GameQuestManager.Instance.currentStep == GameQuestManager.QuestStep.GoToAlexisOffice)
                {
                    alreadyTriggered = true;
                    GameQuestManager.Instance.OnEnteredAlexisOffice();
                }
                break;

            case TriggerType.LibraryExitAfterReturningKey:
                if (GameQuestManager.Instance.currentStep == GameQuestManager.QuestStep.ReturnKeyToLibrary)
                {
                    alreadyTriggered = true;
                    GameQuestManager.Instance.OnReturnedKeyAndExitedLibrary();
                }
                break;

            case TriggerType.CampusExitWin:
                if (GameQuestManager.Instance.currentStep == GameQuestManager.QuestStep.EscapeCampus)
                {
                    alreadyTriggered = true;
                    GameQuestManager.Instance.OnReachedCampusExit();
                }
                break;
        }
    }

    private void HandleLibraryKeyReturn()
    {
        if (GameQuestManager.Instance == null)
        {
            return;
        }

        if (GameQuestManager.Instance.currentStep != GameQuestManager.QuestStep.ReturnKeyToLibrary)
        {
            return;
        }

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.SetBottomInstruction(interactionMessage);
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (onlyOnce && alreadyTriggered)
            {
                return;
            }

            alreadyTriggered = true;

            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ClearBottomInstruction();
            }

            GameQuestManager.Instance.OnReturnedKeyAndExitedLibrary();
        }
    }
}
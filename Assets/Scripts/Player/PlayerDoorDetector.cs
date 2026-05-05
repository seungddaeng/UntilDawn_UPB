using UnityEngine;

public class PlayerDoorDetector : MonoBehaviour
{
    [SerializeField] float interactRange = 5f;

    private DoorInteraction_ currentDoor;
    private bool wasShowingDoorPrompt = false;

    private void Update()
    {
        DetectDoorForPrompt();
    }

    private void DetectDoorForPrompt()
    {
        currentDoor = null;

        if (Camera.main == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit[] hits = Physics.RaycastAll(ray, interactRange);

        foreach (RaycastHit hit in hits)
        {
            DoorInteraction_ door = hit.collider.GetComponentInParent<DoorInteraction_>();

            if (door != null)
            {
                currentDoor = door;
                wasShowingDoorPrompt = true;

                if (UIMessageManager.Instance != null)
                {
                    UIMessageManager.Instance.SetBottomInstruction("Presiona [R] para abrir/cerrar");
                }

                return;
            }
        }

        if (wasShowingDoorPrompt)
        {
            wasShowingDoorPrompt = false;

            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ClearBottomInstruction();
            }
        }
    }

    public void OnOpenDoor()
    {
        if (currentDoor == null)
        {
            DetectDoorForPrompt();
        }

        if (currentDoor == null)
        {
            return;
        }

        if (currentDoor.requiresKey && GameQuestManager.Instance != null && !GameQuestManager.Instance.hasKeys)
        {
            currentDoor.PlayLockedSound();

            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ShowMessage(currentDoor.lockedMessage, 3f);
                UIMessageManager.Instance.ClearBottomInstruction();
            }

            GameQuestManager.Instance.OnAlexisDoorLocked();
            return;
        }

        currentDoor.ToggleDoor();
        currentDoor.PlayOpenSound();
    }
}
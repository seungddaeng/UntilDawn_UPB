using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction;
    public DialogueManager dialogueManager;
    private NPCInteractable currentNPC;

    private void OnEnable()
    {
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
    }

    private void Update()
    {
        DetectNPC();

        if (!interactAction.action.WasPressedThisFrame())
            return;

        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            dialogueManager.NextLine();
            return;
        }
        if (currentNPC != null && currentNPC.CanInteract())
        {
            currentNPC.Interact();
        }
    }

    void DetectNPC()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            currentNPC = hit.collider.GetComponentInParent<NPCInteractable>();
        }
        else
        {
            currentNPC = null;
        }
    }
}
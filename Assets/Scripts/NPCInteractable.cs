using TMPro;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public TMP_Text interactText;

    public float lookRange = 3f;
    public LayerMask npcLayerMask;

    private Camera playerCamera;
    private bool isLookedAt = false;

    private NPCDialogue dialogue;

    private void Awake()
    {
        dialogue = GetComponent<NPCDialogue>();
    }

    public void Interact()
    {
        dialogue?.Interact();
    }

    private void Start()
    {
        playerCamera = Camera.main;
        interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        Debug.DrawRay(
        playerCamera.transform.position,
        playerCamera.transform.forward * lookRange,
        Color.red
        );
        CheckIfPlayerIsLooking();
    }

    void CheckIfPlayerIsLooking()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            NPCInteractable npc = hit.collider.GetComponentInParent<NPCInteractable>();

            if (npc != null && npc == this)
            {
                if (!isLookedAt)
                {
                    isLookedAt = true;
                    interactText.gameObject.SetActive(true);
                    Debug.Log("Player is looking at NPC");
                }
                return;
            }
        }

        if (isLookedAt)
        {
            isLookedAt = false;
            interactText.gameObject.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        return isLookedAt;
    }
}
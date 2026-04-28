using TMPro;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public TMP_Text interactText;

    public float lookRange = 3f;
    public LayerMask npcLayerMask;

    private Camera playerCamera;
    private Transform playerTransform;
    private bool isLookedAt = false;

    private NPCDialogue dialogue;

    private Quaternion originalRotation;
    private bool isInteracting = false;

    [SerializeField] float returnRotationSpeed = 3f;

    private void Awake()
    {
        dialogue = GetComponent<NPCDialogue>();
    }

    public void Interact()
    {
        isInteracting = true;
        LookAtPlayer();
        dialogue?.Interact();
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerTransform = playerCamera.transform;
        interactText.gameObject.SetActive(false);
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        CheckIfPlayerIsLooking();
        if (!isInteracting)
        {
            ReturnToOriginalRotation();
        }
    }

    public void LookAtPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
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
            isInteracting = false;
        }
    }

    void ReturnToOriginalRotation()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            originalRotation,
            returnRotationSpeed * Time.deltaTime
        );
    }

    public bool CanInteract()
    {
        return isLookedAt;
    }
}
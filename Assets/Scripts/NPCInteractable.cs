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

    [Header("Misión")]
    public bool isMarceloQuestNpc = false;

    [Header("Control de mensaje")]
    public float hidePromptAfterInteractTime = 4f;
    private float hidePromptUntil = 0f;

    private void Awake()
    {
        dialogue = GetComponent<NPCDialogue>();
    }

    private void Start()
    {
        playerCamera = Camera.main;

        if (playerCamera != null)
        {
            playerTransform = playerCamera.transform;
        }

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

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

    public void Interact()
    {
        isInteracting = true;
        hidePromptUntil = Time.time + hidePromptAfterInteractTime;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        LookAtPlayer();

        if (isMarceloQuestNpc && GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnMarceloInteracted();
            return;
        }

        dialogue?.Interact();
    }

    public void LookAtPlayer()
    {
        if (playerTransform == null)
        {
            return;
        }

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
        if (playerCamera == null || interactText == null)
        {
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            NPCInteractable npc = hit.collider.GetComponentInParent<NPCInteractable>();

            if (npc != null && npc == this)
            {
                isLookedAt = true;

                bool canShowPrompt = Time.time >= hidePromptUntil;

                if (isMarceloQuestNpc && GameQuestManager.Instance != null)
                {
                    GameQuestManager.QuestStep step = GameQuestManager.Instance.currentStep;

                    canShowPrompt =
                        step == GameQuestManager.QuestStep.FindMarceloFirst ||
                        step == GameQuestManager.QuestStep.ReturnToMarceloSecond ||
                        step == GameQuestManager.QuestStep.ReturnToMarceloThird;
                }

                interactText.gameObject.SetActive(canShowPrompt);
                return;
            }
        }

        isLookedAt = false;
        isInteracting = false;
        interactText.gameObject.SetActive(false);
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
        if (!isLookedAt || Time.time < hidePromptUntil)
        {
            return false;
        }

        if (isMarceloQuestNpc && GameQuestManager.Instance != null)
        {
            GameQuestManager.QuestStep step = GameQuestManager.Instance.currentStep;

            return step == GameQuestManager.QuestStep.FindMarceloFirst ||
                   step == GameQuestManager.QuestStep.ReturnToMarceloSecond ||
                   step == GameQuestManager.QuestStep.ReturnToMarceloThird;
        }

        return true;
    }
}
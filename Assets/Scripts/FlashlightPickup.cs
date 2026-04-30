using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightPickup : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public InputActionReference interactAction;
    public string interactionMessage = "Presiona [E] para recoger la linterna";
    public string collectedMessage = "Linterna conseguida";

    private bool playerInRange = false;
    private FlashlightSystem playerFlashlight;

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
        }
    }

    private void Update()
    {
        if (!playerInRange || interactAction == null)
        {
            return;
        }

        if (interactAction.action.WasPressedThisFrame())
        {
            CollectFlashlight();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        playerFlashlight = other.GetComponentInParent<FlashlightSystem>();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.SetBottomInstruction(interactionMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
        playerFlashlight = null;

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }

    private void CollectFlashlight()
    {
        if (playerFlashlight == null)
        {
            Debug.LogWarning("No se encontró FlashlightSystem en el Player.");
            return;
        }

        playerFlashlight.GiveFlashlight();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage(collectedMessage, 2f);
            UIMessageManager.Instance.ClearBottomInstruction();
        }

        Destroy(gameObject);
    }
}
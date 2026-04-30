using UnityEngine;
using UnityEngine.InputSystem;

public class BatteryPickup : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public InputActionReference interactAction;
    public string interactionMessage = "Presiona [E] para recoger batería";
    public string collectedMessage = "Batería conseguida";

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
            CollectBattery();
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

    private void CollectBattery()
    {
        if (playerFlashlight == null)
        {
            Debug.LogWarning("No se encontró FlashlightSystem en el Player.");
            return;
        }

        playerFlashlight.GiveBattery();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage(collectedMessage, 2f);
            UIMessageManager.Instance.ClearBottomInstruction();
        }

        Destroy(gameObject);
    }
}
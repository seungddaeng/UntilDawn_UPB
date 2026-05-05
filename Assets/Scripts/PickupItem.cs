using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PickupItem : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public string interactionMessage = "Presiona [E] para recoger";
    public string collectedMessage = "Objeto recogido";

    protected bool playerInRange = false;
    protected bool collected = false;

    protected FlashlightSystem playerFlashlight;

    private void Update()
    {
        if (!playerInRange || collected)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Collect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        playerFlashlight = other.GetComponentInParent<FlashlightSystem>();

        ShowInteractionMessage();
    }

    private void OnTriggerStay(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;

        if (playerFlashlight == null)
        {
            playerFlashlight = other.GetComponentInParent<FlashlightSystem>();
        }

        ShowInteractionMessage();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }

    private void ShowInteractionMessage()
    {
        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.SetBottomInstruction(interactionMessage);
        }
    }

    private void Collect()
    {
        collected = true;

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ClearBottomInstruction();
            UIMessageManager.Instance.ShowMessage(collectedMessage, 2f);
        }

        OnCollected();

        gameObject.SetActive(false);
    }

    protected abstract void OnCollected();
}
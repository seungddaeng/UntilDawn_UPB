using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PickupItem : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public InputActionReference interactAction;
    public string interactionMessage = "Presiona [E] para recoger";
    public string collectedMessage = "Objeto recogido";

    protected bool playerInRange = false;
    protected bool collected = false;
    protected GameObject playerObject;
    protected FlashlightSystem playerFlashlight;

    protected virtual void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
        }
    }

    protected virtual void Update()
    {
        if (collected)
        {
            return;
        }

        if (!playerInRange || interactAction == null)
        {
            return;
        }

        if (interactAction.action.WasPressedThisFrame())
        {
            Collect();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        playerObject = other.gameObject;
        playerFlashlight = other.GetComponentInParent<FlashlightSystem>();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.SetBottomInstruction(interactionMessage);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
        playerObject = null;
        playerFlashlight = null;

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }

    private void Collect()
    {
        collected = true;

        OnCollected();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage(collectedMessage, 2f);
            UIMessageManager.Instance.ClearBottomInstruction();
        }

        Destroy(gameObject);
    }

    protected abstract void OnCollected();
}
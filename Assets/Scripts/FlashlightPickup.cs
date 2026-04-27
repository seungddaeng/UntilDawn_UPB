using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightPickup : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            FlashlightSystem playerFlashlight = FindAnyObjectByType<FlashlightSystem>();

            if (playerFlashlight != null)
            {
                playerFlashlight.GiveFlashlight();
                UIMessageManager.Instance?.ShowMessage("¡Linterna conseguida!", 2f);
                UIMessageManager.Instance?.SetBottomInstruction("Clic izquierdo: encender/apagar");
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UIMessageManager.Instance?.ShowMessage("Presiona [E] para recoger la linterna", 2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
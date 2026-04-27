using UnityEngine;
using UnityEngine.InputSystem;

public class BatteryPickup : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            FlashlightSystem playerFlashlight = FindAnyObjectByType<FlashlightSystem>();

            if (playerFlashlight != null)
            {
                playerFlashlight.GiveBattery();
                UIMessageManager.Instance?.ShowMessage("¡Linterna cargada!", 2f);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UIMessageManager.Instance?.ShowMessage("Presiona [E] para recoger baterías", 2f);
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
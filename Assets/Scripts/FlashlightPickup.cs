using UnityEngine;

public class FlashlightPickup : PickupItem
{
    protected override void OnCollected()
    {
        if (playerFlashlight == null)
        {
            Debug.LogWarning("No se encontró FlashlightSystem en el Player.");
            return;
        }

        playerFlashlight.GiveFlashlight();

        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnFlashlightCollected();
        }
    }
}
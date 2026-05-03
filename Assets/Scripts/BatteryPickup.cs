using UnityEngine;

public class BatteryPickup : PickupItem
{
    protected override void OnCollected()
    {
        if (playerFlashlight == null)
        {
            Debug.LogWarning("No se encontró FlashlightSystem en el Player.");
            return;
        }

        playerFlashlight.GiveBattery();

        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnBatteryCollected();
        }
    }
}
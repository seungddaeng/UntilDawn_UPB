using UnityEngine;

public class QuestPickup : PickupItem
{
    public enum PickupType
    {
        Flashlight,
        Battery,
        Key,
        Exam
    }

    [Header("Tipo de objeto")]
    public PickupType pickupType;

    [Header("Referencias para llave")]
    public KeySlot keySlot;

    protected override void OnCollected()
    {
        switch (pickupType)
        {
            case PickupType.Flashlight:
                CollectFlashlight();
                break;

            case PickupType.Battery:
                CollectBattery();
                break;

            case PickupType.Key:
                CollectKey();
                break;

            case PickupType.Exam:
                CollectExam();
                break;
        }
    }

    private void CollectFlashlight()
    {
        if (playerFlashlight != null)
        {
            playerFlashlight.GiveFlashlight();
        }

        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnFlashlightCollected();
        }

        Debug.Log("Linterna recogida");
    }

    private void CollectBattery()
    {
        if (playerFlashlight != null)
        {
            playerFlashlight.GiveBattery();
        }

        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnBatteryCollected();
        }

        Debug.Log("Batería recogida");
    }

    private void CollectKey()
    {
        if (keySlot != null)
        {
            keySlot.RemoveKey();
        }

        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnKeysCollected();
        }

        Debug.Log("Llave recogida");
    }

    private void CollectExam()
    {
        if (GameQuestManager.Instance != null)
        {
            GameQuestManager.Instance.OnExamCollected();
        }

        Debug.Log("Examen recogido");
    }
}
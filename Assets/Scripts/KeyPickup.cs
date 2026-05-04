using UnityEngine;

public class KeyPickup : PickupItem
{
    public KeySlot keySlot;
    public PlayerInventory playerInventory;

    protected override void OnCollected()
    {
        if (keySlot != null)
        {
            keySlot.RemoveKey();
        }

        if (playerInventory != null)
        {
            playerInventory.hasKey = true;
        }

        Debug.Log("Player robó la llave");
    }
}
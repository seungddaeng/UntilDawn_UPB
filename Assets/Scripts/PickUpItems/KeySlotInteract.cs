using UnityEngine;
using UnityEngine.InputSystem;

public class KeySlotInteract : MonoBehaviour
{
    [Header("References")]
    public KeySlot keySlot;
    public PlayerInventory playerInventory;
    public InputActionReference interactAction;

    private bool playerInRange;

    void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    void Update()
    {
        if (!playerInRange) return;
        if (!playerInventory.hasKey) return;
        if (keySlot.hasKey) return;

        if (interactAction.action.triggered)
        {
            ReturnKey();
        }
    }

    void ReturnKey()
    {
        keySlot.PlaceKey();
        playerInventory.hasKey = false;

        Debug.Log(" Player devolvió la llave");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entró al rango del KeySlot");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player salió del rango del KeySlot");
        }
    }
}
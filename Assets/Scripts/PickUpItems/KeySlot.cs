using UnityEngine;

public class KeySlot : MonoBehaviour
{
    public bool hasKey = true;

    public void PlaceKey()
    {
        hasKey = true;
        Debug.Log("La llave fue devuelta a su lugar");
    }

    public void RemoveKey()
    {
        hasKey = false;
        Debug.Log("La llave fue retirada");
    }
}
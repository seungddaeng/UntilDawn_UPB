using UnityEngine;

public class LevelStartMessage : MonoBehaviour
{
    void Start()
    {
        UIMessageManager.Instance?.ShowMessage("¡Que no te atrapen! Consigue la linterna", 4f);
    }
}
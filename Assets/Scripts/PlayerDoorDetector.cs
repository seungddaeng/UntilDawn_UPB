using UnityEngine;

public class PlayerDoorDetector : MonoBehaviour
{
    [SerializeField] float interactRange = 5f; // subí el rango por si acaso

    public void OnOpenDoor()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red, 3f);

        // Raycast contra TODO, sin filtros
        RaycastHit[] hits = Physics.RaycastAll(ray, interactRange);

        Debug.Log("Objetos golpeados: " + hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("  → " + hit.collider.gameObject.name);
            DoorInteraction_ door = hit.collider.GetComponentInParent<DoorInteraction_>();
            if (door != null)
            {
                door.ToggleDoor();
                return;
            }
        }

        Debug.Log("Ningún objeto tenía DoorInteraction_");
    }
}
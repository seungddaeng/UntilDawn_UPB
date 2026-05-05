using UnityEngine;

public class DoorInteraction_ : MonoBehaviour
{
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openSpeed = 5f;
    [SerializeField] bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool moving = false;

    [Header("Configuración de llave")]
    public bool requiresKey = false;
    public string lockedMessage = "No tienes la llave de Alexis, busca al Inge Marcelo para que te ayude.";

    [Header("Audio")]
    public AudioSource openDoorAudio;
    public AudioSource lockedDoorAudio;

    void Start()
    {
        Transform pivot = transform.parent;
        closedRotation = pivot.rotation;
        openRotation = Quaternion.Euler(pivot.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        moving = true;
    }

    public void PlayOpenSound()
    {
        if (openDoorAudio != null)
        {
            openDoorAudio.Play();
        }
    }

    public void PlayLockedSound()
    {
        if (lockedDoorAudio != null)
        {
            lockedDoorAudio.Play();
        }
    }

    void Update()
    {
        if (!moving) return;

        Transform pivot = transform.parent;
        Quaternion target = isOpen ? openRotation : closedRotation;

        pivot.rotation = Quaternion.Slerp(pivot.rotation, target, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(pivot.rotation, target) < 0.5f)
        {
            pivot.rotation = target;
            moving = false;
        }
    }
}
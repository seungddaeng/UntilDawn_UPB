using UnityEngine;

public class DoorInteraction_ : MonoBehaviour
{
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openSpeed = 5f;
    [SerializeField] bool isOpen = false;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private bool _moving = false;

    void Start()
    {
        Transform pivot = transform.parent;
        _closedRotation = pivot.rotation;
        _openRotation = Quaternion.Euler(pivot.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        _moving = true;
    }

    void Update()
    {
        if (!_moving) return;

        Transform pivot = transform.parent;
        Quaternion target = isOpen ? _openRotation : _closedRotation;

        pivot.rotation = Quaternion.Slerp(pivot.rotation, target, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(pivot.rotation, target) < 0.5f)
        {
            pivot.rotation = target;
            _moving = false;
        }
    }
}
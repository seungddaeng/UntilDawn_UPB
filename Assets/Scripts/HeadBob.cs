using UnityEngine;

public class HeadBob : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerMovement playerMovement;

    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.04f;
    public float sprintBobSpeed = 18f;
    public float sprintBobAmount = 0.08f;

    private float defaultY;
    private float timer;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        if (rb == null) return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > 0.1f;
        bool isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.05f;

        if (isMoving && isGrounded)
        {
            bool isSprinting = playerMovement != null &&
                               playerMovement.sprintAction.action.IsPressed();

            float speed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            float amount = isSprinting ? sprintBobAmount : walkBobAmount;

            timer += Time.deltaTime * speed;

            float newY = defaultY + Mathf.Sin(timer) * amount;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                newY,
                transform.localPosition.z
            );
        }
        else
        {
            timer = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(transform.localPosition.x, defaultY, transform.localPosition.z),
                Time.deltaTime * 8f
            );
        }
    }
}
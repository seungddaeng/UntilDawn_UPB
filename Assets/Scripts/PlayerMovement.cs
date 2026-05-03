using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 7f;
    public float sprintSpeed = 12f;
    public float jumpForce = 7f;
    public InputActionReference sprintAction;
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip sprintClip;
    public float stepRate = 0.4f;
    public float sprintStepRate = 0.25f;
    private Vector2 movementInput;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isSprinting;
    private float stepTimer;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        Vector3[] directions = { transform.forward, transform.right, -transform.right, transform.up, -transform.up };
        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, 2f))
            {
                Debug.Log("Objeto cerca: " + hit.collider.gameObject.name + " | distancia: " + hit.distance);
            }
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
    public void OnMovement(InputValue data)
    {
        movementInput = data.Get<Vector2>();
    }
    public void OnJump(InputValue data)
    {
        if (!data.isPressed) return;
        if (!isGrounded) return;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
    void MovePlayer()
    {
        isSprinting = sprintAction.action.IsPressed();
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        direction = direction.normalized;
        rb.linearVelocity = new Vector3(
            direction.x * currentSpeed,
            rb.linearVelocity.y,
            direction.z * currentSpeed
        );
        HandleFootsteps(isSprinting, direction);
    }
    private void HandleFootsteps(bool isSprinting, Vector3 direction)
    {
        bool isMoving = direction.magnitude > 0.1f && isGrounded;
        if (isMoving)
        {
            stepTimer -= Time.fixedDeltaTime;
            if (stepTimer <= 0f)
            {
                AudioClip clipToPlay = isSprinting ? sprintClip : walkClip;
                audioSource.PlayOneShot(clipToPlay);
                stepTimer = isSprinting ? sprintStepRate : stepRate;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
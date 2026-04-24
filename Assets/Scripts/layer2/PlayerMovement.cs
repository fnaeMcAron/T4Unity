using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Проверка земли")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck ? groundCheck.position : transform.position,
            groundCheckRadius,
            groundMask
        );

        Vector3 move = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
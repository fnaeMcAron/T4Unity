using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.Rendering.Universal;
using TMPro;

public class StudentMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public bool isDead;

    public float moveX;
    public AnimationPlayer anim;

    [Header("Гены")]
    public Light2D light;
    public float wallWalkSpeed = 4f;
    public int jumpCount;
    public float dashForce = 14f;
    public int maxDashCount = 2;

    [Header("Смерть")]
    public Transform respawnPoint;
    public GameObject deathMenu;
    Vector3 cachedRespawnPos;

    Rigidbody2D rb;
    bool grounded = true;
    Vector2 moveVec;

    [HideInInspector] public bool airControl; // 100
    [HideInInspector] public bool canWallWalk; // 010
    [HideInInspector] public bool canDash; // 110
    [HideInInspector] public bool canActivateElectric; //001
    [HideInInspector] public bool lightEnabled; // 101
    [HideInInspector] public bool canChangeSize; // 011
    [HideInInspector] public bool extraJump; // 111

    bool roomAllowsWallWalk;
    bool WallWalkMode => canWallWalk && roomAllowsWallWalk;

    float baseSpeed;
    float baseJumpForce;
    //Vector3 baseScale;
    float baseGravity;
    int jumpRemains;
    Collider2D col;

    int dashRemains;
    bool isDashing;
    float dashTime = 0.12f;
    float dashTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        cachedRespawnPos = respawnPoint != null
        ? respawnPoint.position
        : transform.position;

        baseSpeed = speed;
        baseJumpForce = jumpForce;
        //baseScale = transform.localScale;
        baseGravity = rb.gravityScale;
        dashRemains = maxDashCount;

        anim = GetComponent<AnimationPlayer>();
    }

    void FixedUpdate()
    {
        if (isDead) return;


        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
                isDashing = false;

            return;
        }

        if (WallWalkMode)
        {
            rb.velocity = moveVec * wallWalkSpeed;
            return;
        }

        if (grounded || airControl)
        {
            rb.velocity = new Vector2(moveVec.x * speed, rb.velocity.y);
        }
    }
    void Update()
    {

        anim.isGrounded = grounded;

        light.enabled = lightEnabled;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        grounded = true;
        jumpRemains = jumpCount;
        dashRemains = maxDashCount;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        rb.velocity = Vector2.zero;
        //rb.gravityScale = 0;
        //rb.simulated = false;

        moveVec = Vector2.zero;

        if (col != null)
            col.enabled = false;

        deathMenu.SetActive(true);
        ResetModifiers();
    }

    public void Respawn()
    {
        isDead = false;

        transform.position = cachedRespawnPos;

        //rb.simulated = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = baseGravity;

        if (col != null)
            col.enabled = true;

        jumpRemains = jumpCount;
        dashRemains = maxDashCount;
        grounded = false;
        isDashing = false;

        deathMenu.SetActive(false);
    }

    public void SetRespawnPoint(Transform point)
    {
        cachedRespawnPos = point.position;
    }

    void UpdateWallWalkState()
    {
        rb.gravityScale = WallWalkMode ? 0 : baseGravity;
    }

    public void SetRoomSpace(bool allow)
    {
        roomAllowsWallWalk = allow;
        UpdateWallWalkState();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (isDead) return;

        moveVec = ctx.ReadValue<Vector2>();

        if (moveVec == Vector2.left)
            GetComponent<SpriteRenderer>().flipX = true;
        else 
            GetComponent<SpriteRenderer>().flipX = false;

        anim.moveX = moveVec.x;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (isDead)
        {
            Respawn();
            return;
        }

        if (canWallWalk && roomAllowsWallWalk) return;

        if (grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            grounded = false;
            anim.PlayJump();
        }
        else if (extraJump && jumpRemains > -1)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRemains--;
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (isDead) return;
        if (!ctx.started) return;
        if (!canDash) return;
        if (dashRemains <= 0) return;
        if (WallWalkMode) return;

        float dir = moveVec.x;

        if (Mathf.Abs(dir) < 0.1f)
            dir = GetComponent<SpriteRenderer>().flipX == true ? -1 : 1;

        rb.velocity = new Vector2(dir * dashForce, 0f);

        dashRemains--;
        isDashing = true;
        dashTimer = dashTime;
    }

    public void ResetModifiers()
    {
        speed = baseSpeed;
        jumpForce = baseJumpForce;
        //transform.localScale = baseScale;

        airControl = false;
        canWallWalk = false;
        canDash = false;
        extraJump = false;
        canChangeSize = false;
        canActivateElectric = false;
        lightEnabled = false;

        UpdateWallWalkState();
    }
}
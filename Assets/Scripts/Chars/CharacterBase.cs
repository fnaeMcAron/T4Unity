using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Информация о персонаже")]
    public string charname;

    [Header("Компоненты")]
    protected Rigidbody rb;
    protected PlayerInput playerInput;
    protected Animator animator;
    [SerializeField] protected GameObject cameraPivot;
    [SerializeField] protected CameraFollow cameraFollow;

    [Header("Настройки")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Боевые настройки")]
    public float baseDamage = 10f;
    public float attackInterval = 0.5f;
    public float meleeRange = 2f;
    public float rangedRange = 10f;

    [Header("Текущие баффы")]
    public MusicBuff activeMusicBuff;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;

    protected Vector2 moveInput;
    protected Vector2 lookInput;
    private Vector3 movement;
    private bool isGrounded;
    private Transform cameraTransform;

    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int isIdleHash = Animator.StringToHash("isIdle");

    // Таймер для анимации танца
    private float idleTimer = 0f;
    private readonly float danceTriggerTime = 10f; // 10 секунд до танца
    private bool isIdle = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
        /*
        if (playerInput != null)
        {
            playerInput.enabled = false;
        } */
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            if (isIdle)
            {
                isIdle = false;
                animator.SetBool(isIdleHash, false);
                Debug.Log("Танцы прекращены - персонаж движется");
            }
            idleTimer = 0f;
            animator.SetBool(isMovingHash, true);
        }
        else
        {
            animator.SetBool(isMovingHash, false);

            // Если не движемся и не танцуем, увеличиваем таймер
            if (!isIdle)
            {
                idleTimer += Time.deltaTime;

                // Проверяем, прошло ли 10 секунд бездействия
                if (idleTimer >= danceTriggerTime)
                {
                    StartDancing();
                }
            }
        }

        Debug.Log($"Движение: {isMoving}, Таймер: {idleTimer:F1}, Танец: {isIdle}");
    }

    private void StartDancing()
    {
        isIdle = true;
        animator.SetBool(isIdleHash, true);
        Debug.Log("Включаем анимацию танца!");
    }

    // Метод для принудительного прекращения танца (например, при атаке)
    public void StopDancing()
    {
        if (isIdle)
        {
            isIdle = false;
            idleTimer = 0f;
            animator.SetBool(isIdleHash, false);
            Debug.Log("Танец принудительно остановлен");
        }
    }

    void Update()
    {
        Move();
        UpdateAnimations();
    }

    public virtual void ApplyMusicBuff(MusicBuff buff)
    {
        activeMusicBuff = buff;
        damageMultiplier = buff.damageMultiplier;
        speedMultiplier = buff.moveSpeedMultiplier;
        attackSpeedMultiplier = buff.attackSpeedMultiplier;

        Debug.Log($"{name} получил бафф: {buff.buffName}");
    }

    public virtual void ResetBuffs()
    {
        activeMusicBuff = null;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        attackSpeedMultiplier = 1f;
    }

    public virtual void OnCharacterSelected()
    {
        if (playerInput != null)
            playerInput.enabled = true;
    }

    public virtual void OnCharacterDeselected()
    {
        if (playerInput != null)
            playerInput.enabled = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMeleeAttack(InputValue value)
    {
        if (value.isPressed)
            PerformMeleeAttack();
    }

    public void OnRangedAttack(InputValue value)
    {
        if (value.isPressed)
            PerformRangedAttack();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            Jump();
    }

    public void OnDodge(InputValue value)
    {
        if (value.isPressed)
            Dodge();
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UseAbility(true);
        }
        else if (context.canceled)
        {
            UseAbility(false);
        }
        Debug.Log(context.phase);
    }

    private void Move()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
            else
                return;
        }

        // движение относительно камеры (очень странное, я сам ничего не понял, потом поменяю)
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1));
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1));

        movement = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        float currentMoveSpeed = moveSpeed * speedMultiplier;
        Vector3 targetVelocity = movement * currentMoveSpeed;
        targetVelocity.y = rb.velocity.y;

        rb.velocity = targetVelocity;

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public virtual void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            float angle = Vector3.Angle(collision.contacts[0].normal, Vector3.up);
            if (angle < 45f)
            {
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public abstract void PerformMeleeAttack();
    public abstract void PerformRangedAttack();
    public abstract void UseAbility(bool isHold);
    public abstract void Dodge();
}
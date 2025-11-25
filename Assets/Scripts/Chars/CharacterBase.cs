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
    [SerializeField] protected CameraFollow cameraFollow;

    [Header("Настройки")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Боевые настройки")]
    public WeaponSlot[] weaponSlots = new WeaponSlot[2];
    public int currentWeaponIndex = 0; // 0-ближнее, 1-дальнее

    [System.Serializable]
    public class WeaponSlot
    {
        public string slotName;
        public GameObject weaponObject;
        public bool isAvailable = true;
    }

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
    private readonly int isJumpHash = Animator.StringToHash("Jump");
    private readonly int firstMeleeAttackTriggerHash = Animator.StringToHash("FirstMeleeAttack");
    private readonly int secondMeleeAttackTriggerHash = Animator.StringToHash("SecondMeleeAttack");
    private readonly int rangedAttackHash = Animator.StringToHash("RangedAttack");
    private readonly int rangedIdleHash = Animator.StringToHash("RangedIdle");

    public GameObject meleeModel;
    public GameObject rangedModel;

    // Переменные для управления атакой
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    private int attackCount = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        FindActiveAnimator();
        animator = GetComponent<Animator>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        InitializeWeapons();
    }

    private void FindActiveAnimator()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found in character model!");
        }
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponObject != null)
            {
                weaponSlots[i].weaponObject.SetActive(i == currentWeaponIndex);
            }
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            animator.SetBool(isMovingHash, true);
        }
        else
        {
            animator.SetBool(isMovingHash, false);
        }

        // Автоматически выключаем анимацию прыжка при приземлении
        if (isGrounded && animator.GetBool(isJumpHash))
        {
            animator.SetBool(isJumpHash, false);
        }
    }

    void Update()
    {
        Move();
        UpdateAnimations();

        // Проверяем кулдаун атаки
        if (Time.time - lastAttackTime >= attackInterval)
        {
            canAttack = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.canceled && canAttack)
        {
            if (currentWeaponIndex == 0)
            {
                // Генерируем случайную атаку от 1 до 2
                int attackIndex = Random.Range(1, 3);

                // Запускаем соответствующую анимацию атаки
                if (attackIndex == 1)
                {
                    animator.SetTrigger(firstMeleeAttackTriggerHash);
                    Debug.Log("Запущена первая анимация атаки!");
                }
                else if (attackIndex == 2)
                {
                    animator.SetTrigger(secondMeleeAttackTriggerHash);
                    Debug.Log("Запущена вторая анимация атаки!");
                }

                PerformMeleeAttack(false);
            }
            else if (currentWeaponIndex == 1)
            {
                if (animator == null)
                {
                    Debug.LogError("Animator is null in PlayRangedAnimation!");
                    return;
                }

                animator.SetTrigger(rangedAttackHash);
                PerformRangedAttack(false);
            }

            // Устанавливаем кулдаун
            canAttack = false;
            lastAttackTime = Time.time;
        } else if (context.performed)
        {
            if (currentWeaponIndex == 0)
            {
                PerformMeleeAttack(true);
            }
            else if (currentWeaponIndex == 1)
            {
                PerformRangedAttack(true);
            }
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Dodge();
        }
        else if (context.canceled)
        {
            // Пока не трогать
            Dodge();
            //Riding();
        }
    }


    public void OnCameraAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleTargetLock();
        }
        else if (context.canceled)
        {
            ResetCamera();
        }
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue != Vector2.zero)
            {
                SwitchWeapon(scrollValue);
            }
        }
    }

    private void SwitchWeapon(Vector2 direction)
    {
        int newIndex = currentWeaponIndex;
        do
        {
            newIndex = (newIndex + (int)direction.y + weaponSlots.Length) % weaponSlots.Length;
        }
        while (!weaponSlots[newIndex].isAvailable && newIndex != currentWeaponIndex);

        if (weaponSlots[newIndex].isAvailable && newIndex != currentWeaponIndex)
        {
            SetCurrentWeapon(newIndex);
        }
    }

    private void SetCurrentWeapon(int newIndex)
    {
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            weaponSlots[currentWeaponIndex].weaponObject.SetActive(false);
        }

        currentWeaponIndex = newIndex;
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            weaponSlots[currentWeaponIndex].weaponObject.SetActive(true);
        }

        SwitchCharacterModel();
        Debug.Log($"Переключено на оружие: {weaponSlots[currentWeaponIndex].slotName}");
    }

    private void SwitchCharacterModel()
    {
        if (meleeModel == null || rangedModel == null) return;

        bool isMeleeWeapon = currentWeaponIndex == 0;
        meleeModel.SetActive(isMeleeWeapon);
        rangedModel.SetActive(!isMeleeWeapon);

        // Обновляем аниматор при смене модели
        Animator newAnimator = isMeleeWeapon ?
            meleeModel.GetComponent<Animator>() :
            rangedModel.GetComponent<Animator>();

        if (newAnimator != null)
        {
            animator = newAnimator;
        }
    }

    public void SetWeaponByIndex(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index].isAvailable)
        {
            SetCurrentWeapon(index);
        }
    }

    public void SetWeaponAvailable(int index, bool available)
    {
        if (index >= 0 && index < weaponSlots.Length)
        {
            weaponSlots[index].isAvailable = available;
            if (!available && currentWeaponIndex == index)
            {
                SwitchToFirstAvailableWeapon();
            }
        }
    }

    private void SwitchToFirstAvailableWeapon()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].isAvailable)
            {
                SetCurrentWeapon(i);
                return;
            }
        }
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

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            Jump();
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
            if (animator != null)
            {
                animator.SetBool(isJumpHash, true);
            }
        }
    }

    public virtual void ToggleTargetLock()
    {
        // потом
    }

    public virtual void ResetCamera()
    {
        cameraFollow.ResetCameraBehindTarget();
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

    //todo пересмотреть
    public abstract void PerformMeleeAttack(bool isHold);
    public abstract void PerformMeleeChargeAttack();
    public abstract void PerformRangedAttack(bool isHold);
    public abstract void PerformRangedAim();
    public abstract void UseAbility(bool isHold);
    protected abstract void Dodge();
    //public abstract void Riding();
}
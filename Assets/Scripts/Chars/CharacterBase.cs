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
    private readonly int isIdleHash = Animator.StringToHash("isIdle");
    //private readonly int weaponTypeHash = Animator.StringToHash("WeaponType");

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

        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        // Активируем начальное оружие, деактивируем остальные
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponObject != null)
            {
                weaponSlots[i].weaponObject.SetActive(i == currentWeaponIndex);
            }
        }

        //UpdateWeaponAnimations();
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

        //Debug.Log($"Движение: {isMoving}, Таймер: {idleTimer:F1}, Танец: {isIdle}");
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

        // Циклическая смена по массиву
        do
        {
            newIndex = (newIndex + (int)direction.y + weaponSlots.Length) % weaponSlots.Length;
        }
        while (!weaponSlots[newIndex].isAvailable && newIndex != currentWeaponIndex);

        // Если нашли доступное оружие
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

        //UpdateWeaponAnimations();

        Debug.Log($"Переключено на оружие: {weaponSlots[currentWeaponIndex].slotName}");
    }

    /*
    private void UpdateWeaponAnimations()
    {
        if (animator != null)
        {
            // 0 - ближнее оружие, 1 - дальнее оружие
            animator.SetInteger(weaponTypeHash, currentWeaponIndex);
        }
    }*/

    // Метод для принудительной установки оружия по индексу
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentWeaponIndex == 0)
            {
                PerformMeleeAttack();
            }
            else if (currentWeaponIndex == 1)
            {
                PerformRangedAttack();
            }
        }
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

        // движение относительно камеры (каким образом я сам не понимаю)
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
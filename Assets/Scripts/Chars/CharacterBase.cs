using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Информация о персонаже")]
    public string charname;

    [Header("Компоненты")]
    protected Rigidbody rb;
    protected PlayerInput playerInput;
    public Animator animator;
    [SerializeField] protected StyleManager styleManager;
    [SerializeField] protected CameraFollow cameraFollow;
    [SerializeField] protected CharacterManager charMan;

    [Header("Настройки")]
    public bool available = true;
    public GameObject meleeModel;
    public GameObject rangedModel;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Боевые настройки")]
    public WeaponSlot[] weaponSlots = new WeaponSlot[2];
    public int currentWeaponIndex = 0; // 0-ближнее, 1-дальнее
    public float comboTimeWindow = 2f;
    public int maxComboCount = 5;
    //public float attackInterval = 0.5f;
    public float abilityDuration = 1f;

    [System.Serializable]
    public class WeaponSlot
    {
        public string slotName;
        public GameObject[] weaponObject = new GameObject[2];
        public SkinnedMeshRenderer[] skinnedMesh = new SkinnedMeshRenderer[2];
        public float baseDamage = 10f;
        public float range = 2f;
        public bool isAvailable = true;
    }

    [Header("Текущие баффы")]
    public MusicBuff activeMusicBuff;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;

    public Vector2 moveInput;
    protected Vector2 lookInput;
    protected Enemy enemy;
    Vector3 movement;
    public bool isGrounded;
    Transform cameraTransform;

    readonly int isMovingHash = Animator.StringToHash("IsMoving");
    readonly int isJumpHash = Animator.StringToHash("Jump");
    public readonly int firstMeleeAttackTriggerHash = Animator.StringToHash("FirstMeleeAttack");
    public readonly int secondMeleeAttackTriggerHash = Animator.StringToHash("SecondMeleeAttack");
    public readonly int rangedAttackHash = Animator.StringToHash("RangedAttack");
    readonly int rangedIdleHash = Animator.StringToHash("RangedIdle");

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponObject != null)
            {
                for (int j = 0; j < weaponSlots[i].weaponObject.Length; j++)
                {
                    if (weaponSlots[i].weaponObject[j] != null)
                    {
                        weaponSlots[i].weaponObject[j].SetActive(i == currentWeaponIndex);
                    }
                }
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

        if (weaponSlots[currentWeaponIndex].skinnedMesh != null)
        {
            for (int i = 0; i < weaponSlots[currentWeaponIndex].weaponObject.Length; i++)
            {
                if (weaponSlots[currentWeaponIndex].weaponObject[i] != null &&
                    i < weaponSlots[currentWeaponIndex].skinnedMesh.Length &&
                    weaponSlots[currentWeaponIndex].skinnedMesh[i] != null)
                {
                    weaponSlots[currentWeaponIndex].weaponObject[i].transform.position =
                        weaponSlots[currentWeaponIndex].skinnedMesh[i].bounds.center;
                }
            }
        }
    }

    public void SwitchWeapon(Vector2 direction)
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
        // Выключаем все объекты текущего оружия
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            for (int i = 0; i < weaponSlots[currentWeaponIndex].weaponObject.Length; i++)
            {
                if (weaponSlots[currentWeaponIndex].weaponObject[i] != null)
                {
                    weaponSlots[currentWeaponIndex].weaponObject[i].SetActive(false);
                }
            }
        }

        currentWeaponIndex = newIndex;

        // Включаем все объекты нового оружия
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            for (int i = 0; i < weaponSlots[currentWeaponIndex].weaponObject.Length; i++)
            {
                if (weaponSlots[currentWeaponIndex].weaponObject[i] != null)
                {
                    weaponSlots[currentWeaponIndex].weaponObject[i].SetActive(true);
                }
            }
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
        Move();
    }

    public virtual void OnCharacterDeselected()
    {
        if (playerInput != null)
            playerInput.enabled = false;
    }


    public void Move()
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
        else
        {
            rb.angularVelocity = Vector3.zero;
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
        // todo
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

    //todo сделать привязку секунд к длительности анимаций
    protected IEnumerator EnablingCollider(float seconds, int weaponIndex, int points = 0, string actionName = "", int colliderIndex = 0)
    {
        if (weaponSlots[weaponIndex].weaponObject != null &&
            colliderIndex < weaponSlots[weaponIndex].weaponObject.Length &&
            weaponSlots[weaponIndex].weaponObject[colliderIndex] != null)
        {
            Collider collider = weaponSlots[weaponIndex].weaponObject[colliderIndex].GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
                styleManager.AddStylePoints(points, actionName);
                yield return new WaitForSeconds(seconds);
                collider.enabled = false;
            }
        }
    }

    public abstract void PerformMeleeAttack();
    public abstract void PerformMeleeChargeAttack();
    public abstract void PerformRangedAttack();
    public abstract void PerformRangedAim();
    public abstract void UseAbility(bool isHold);
    public abstract void Dodge();
    //public abstract void Riding();
}
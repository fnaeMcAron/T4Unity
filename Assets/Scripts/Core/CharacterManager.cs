using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    [Header("Ссылки")]
    public WormManager wormManager;
    public CharacterBase[] characters;
    public TMP_Text text;
    public CameraFollow cameraFollow;
    public StyleManager styleManager;
    public TMP_Text DEBUG;

    public IdleState idleState = new IdleState();
    public MidairState midairState = new MidairState();
    public AbilityState abilityState = new AbilityState();
    public AttackState attackState = new AttackState(true);
    public DodgeState dodgeState = new DodgeState(true);
    public HoldenAbilityState holdenAbilityState = new HoldenAbilityState();
    public HoldenAttackState holdenAttackState = new HoldenAttackState(true);
    public HoldenDodgeState holdenDodgeState = new HoldenDodgeState();

    [Header("Текущие данные")]
    public StateBase currentState;
    public CharacterBase currentCharacter;
    [SerializeField] private int currentCharacterIndex;

    public CharacterBase CurrentCharacter => currentCharacter;
    public int CurrentCharacterIndex => currentCharacterIndex;

    public delegate void DeathAction();
    public static event DeathAction OnDeath;

    int unchargedAttackCount = 0;
    float lastAttackTime = 0f;

    void Start()
    {
        if (characters.Length > 0)
        {
            //SwitchToCharacter(0);
            currentState = idleState;
            currentState.Enter(this);
            //state.character = currentCharacter;
        }
        Application.targetFrameRate = 8000;
    }

    private void Update()
    {
        currentState.Update(this);
        DEBUG.text = "DB: " + currentState + " " + currentCharacter.isGrounded;
    }

    private void LateUpdate()
    {
        text.text = "ЧЕРВЯЧКИИИ: " + wormManager.GetWorms();
        if (wormManager.GetWorms() <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(this.gameObject);
    }

    public void SwitchToCharacter(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        Vector2 savedMoveInput = currentCharacter.moveInput;
        Debug.Log(currentCharacter.moveInput);
        Vector3 prevPos = currentCharacter.transform.localPosition;
        Quaternion prevRot = currentCharacter.transform.localRotation;
        Vector3 prevVel = currentCharacter.GetComponent<Rigidbody>().velocity;

        //if (currentCharacter != null)
        //{
            currentCharacter.OnCharacterDeselected();
            currentCharacter.gameObject.SetActive(false);
        //}

        currentCharacterIndex = index;
        currentCharacter = characters[index];
        //currentState.character = currentCharacter;

        currentCharacter.transform.localPosition = prevPos;
        currentCharacter.transform.localRotation = prevRot;
        currentCharacter.GetComponent<Rigidbody>().velocity = prevVel;
        currentCharacter.moveInput = savedMoveInput;

        currentCharacter.gameObject.SetActive(true);
        currentCharacter.OnCharacterSelected();

        cameraFollow?.SetTarget(currentCharacter.transform);

        if (currentCharacter is RodionController)
        {
            styleManager?.SwitchToRodionStyle();
        }
        else if (currentCharacter is FinaController)
        {
            styleManager?.SwitchToFinaStyle();
        }
    }

    public int GetCurrentComboCount()
    {
        if (Time.time - lastAttackTime > currentCharacter.comboTimeWindow)
        {
            unchargedAttackCount = 0;
        }
        return unchargedAttackCount;
    }

    public void ResetCombo()
    {
        unchargedAttackCount = 0;
        Debug.Log("Комбо сброшен");
    }

    public bool IsComboActive()
    {
        return unchargedAttackCount > 0 && (Time.time - lastAttackTime) <= currentCharacter.comboTimeWindow;
    }

    public void SwitchState(StateBase nextState)
    {
        currentState.Exit(this);
        currentState = nextState;
        //currentState.character = currentCharacter;
        currentState.Enter(this);
    }






    //для ввода

    public void OnMove(InputAction.CallbackContext context)
    {
        currentCharacter.moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            SwitchState(midairState);
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchState(holdenAbilityState);
        }
        else if (context.canceled)
        {
            SwitchState(abilityState);
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchState(dodgeState);
        }
        else if (context.canceled)
        {
            SwitchState(holdenDodgeState);
        }
    }

    public void OnCameraAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentCharacter.ToggleTargetLock();
        }
        else if (context.canceled)
        {
            currentCharacter.ResetCamera();
        }
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue != Vector2.zero)
            {
                currentCharacter.SwitchWeapon(scrollValue);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            SwitchState(new AttackState(!currentCharacter.isGrounded));
        else if (context.canceled)
            SwitchState(new HoldenAttackState(!currentCharacter.isGrounded));

        if (context.performed)
        {
            if (Time.time - lastAttackTime > currentCharacter.comboTimeWindow)
            {
                unchargedAttackCount = 0;
            }

            unchargedAttackCount++;
            lastAttackTime = Time.time;

            if (unchargedAttackCount > currentCharacter.maxComboCount)
            {
                unchargedAttackCount = 0;
            }

            Debug.Log($"Комбо: {unchargedAttackCount} незаряженных атак");

            if (currentCharacter.currentWeaponIndex == 0)
            {
                // Генерируем случайную атаку от 1 до 2
                int attackIndex = Random.Range(1, 3);

                // Запускаем соответствующую анимацию атаки
                if (attackIndex == 1)
                {
                    currentCharacter.animator.SetTrigger(currentCharacter.firstMeleeAttackTriggerHash);
                }
                else if (attackIndex == 2)
                {
                    currentCharacter.animator.SetTrigger(currentCharacter.secondMeleeAttackTriggerHash);
                }

                currentCharacter.PerformMeleeAttack();
            }
            else if (currentCharacter.currentWeaponIndex == 1)
            {
                currentCharacter.animator.SetTrigger(currentCharacter.rangedAttackHash);
                currentCharacter.PerformRangedAttack();
            }

            lastAttackTime = Time.time;
        }
        else if (context.canceled)
        {
            if (currentCharacter.currentWeaponIndex == 0)
            {
                currentCharacter.PerformMeleeChargeAttack();
            }
            else if (currentCharacter.currentWeaponIndex == 1)
            {
                currentCharacter.PerformRangedAim();
            }
        }
    }
}
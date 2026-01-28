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
    public AttackState attackState = new AttackState();
    public DodgeState dodgeState = new DodgeState(true);
    public HoldenAbilityState holdenAbilityState = new HoldenAbilityState();
    public HoldenAttackState holdenAttackState = new HoldenAttackState();
    public HoldenDodgeState holdenDodgeState = new HoldenDodgeState();

    [Header("Текущие данные")]
    public StateBase currentState;
    public CharacterBase currentCharacter;
    [SerializeField] private int currentCharacterIndex;

    [Header("Настройки для интеракций")]
    string interactTag = "Interactable";
    [SerializeField] private float interactRadius = 10f;


    public CharacterBase CurrentCharacter => currentCharacter;
    public int CurrentCharacterIndex => currentCharacterIndex;

    public delegate void DeathAction();
    public static event DeathAction OnDeath;

    public int unchargedAttackCount = 0;
    public float lastAttackTime = 0f;
    Interactable closestInteractable = null;

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
        CheckDistanceToObjects();
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
        if (PauseManager.Instance.IsGamePaused())
            return;

        if (index < 0 || index >= characters.Length) return;

        Vector2 savedMoveInput = currentCharacter.moveInput;
        //Debug.Log(currentCharacter.moveInput);
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

    void CheckDistanceToObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(currentCharacter.transform.position, interactRadius);
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = currentCharacter.transform.position;

        // Сначала собираем все interactable объекты и находим ближайший
        foreach (Collider col in colliders)
        {
            Interactable script = col.GetComponent<Interactable>();

            if (script != null)
            {
                float distance = Vector3.Distance(playerPosition, col.transform.position);

                if (distance <= interactRadius)
                {
                    // Сохраняем самый близкий объект
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = script;
                    }
                }
            }
        }

        // Теперь проходим еще раз и применяем логику отображения
        foreach (Collider col in colliders)
        {
            Interactable script = col.GetComponent<Interactable>();

            if (script != null)
            {
                float distance = Vector3.Distance(playerPosition, col.transform.position);

                if (distance <= interactRadius)
                {
                    // Проверяем, является ли этот объект ближайшим
                    bool isClosest = (script == closestInteractable);
                    script.DrawGUI(distance, isClosest);
                }
                else
                {
                    script.DeactivateDebug();
                    closestInteractable = null;
                }
            }
        }
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (closestInteractable != null)
                closestInteractable.OnInteract();
            else
                Debug.Log("Ближайшего нет");
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
        if (PauseManager.Instance.IsGamePaused())
            return;

        if (context.performed)
            SwitchState(attackState);
        else if (context.canceled)
            SwitchState(holdenAttackState);
    }
}
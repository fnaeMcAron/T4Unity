using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Основные настройки")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float smoothSpeed = 0.1f;

    [Header("Вращение камеры")]
    public float rotationSpeed = 2f;
    public float verticalAngleLimit = 80f;
    public bool invertY = false;

    [Header("Дистанция камеры")]
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Смена персонажа")]
    public bool preserveCameraOrientation = true;

    // Система ввода
    private TInputControls cameraInput;
    private InputAction lookAction;
    private InputAction zoomAction;
    private InputAction cursorAction;

    // Приватные переменные
    private Vector3 velocity = Vector3.zero;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private float currentDistance;
    private bool isCursorLocked = true;

    void Awake()
    {
        cameraInput = new TInputControls();
    }

    void Start()
    {
        currentDistance = -offset.z;
        InitializeCamera();
        SetupInputActions();
        FindPlayer();
    }

    void InitializeCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            if (currentRotationY == 0 && currentRotationX == 0)
            {
                Vector3 direction = transform.position - target.position;
                currentRotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                currentRotationX = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg;
            }
        }
    }

    void SetupInputActions()
    {
        lookAction = cameraInput.Player.Look;
    }

    void OnEnable()
    {
        lookAction?.Enable();
        cameraInput?.Enable();
    }

    void OnDisable()
    {
        lookAction?.Disable();
        cameraInput?.Disable();
    }

    void Update()
    {
        HandleCameraRotation();
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }
    }

    void HandleCameraRotation()
    {
        if (!target || !isCursorLocked) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * rotationSpeed * 0.1f;
        float mouseY = lookInput.y * rotationSpeed * 0.1f * (invertY ? 1 : -1);

        currentRotationY += mouseX;
        currentRotationX += mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, -verticalAngleLimit, verticalAngleLimit);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -currentDistance);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.LookAt(target.position + Vector3.up * offset.y);
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SetTarget(player.transform);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (target == newTarget) return;

        target = newTarget;
        velocity = Vector3.zero;
    }

    public void ResetCameraBehindTarget()
    {
        if (target != null)
        {
            currentRotationY = target.eulerAngles.y;
            currentRotationX = 20f;
        }
    }

    public void SetRotation(float x, float y)
    {
        currentRotationX = Mathf.Clamp(x, -verticalAngleLimit, verticalAngleLimit);
        currentRotationY = y;
    }

    public void SetDistance(float distance)
    {
        currentDistance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    public void SetCursorLock(bool locked)
    {
        isCursorLocked = locked;
        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
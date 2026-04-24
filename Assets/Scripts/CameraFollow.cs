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
    public float gamepadMultiplier = 50f;
    public float verticalAngleLimit = 80f;
    public bool invertY = false;

    [Header("Дистанция камеры")]
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Коллизии камеры")]
    public LayerMask collisionMask = 0;
    public float cameraRadius = 0.3f;
    public float collisionOffset = 0.2f;
    public float minWallDistance = 0.1f;

    [Header("Экстренные позиции")]
    public bool enableFallbackPositions = true;
    public float[] fallbackAngles = { 0f, 30f, -30f, 45f, -45f };
    public float fallbackDistanceMultiplier = 0.7f;
    public float emergencyForwardOffset = 0.5f;

    [Header("Смена персонажа")]
    public bool preserveCameraOrientation = true;

    [Header("Debug")]
    public bool debugVisualization = false;

    // Система ввода
    private Controls cameraInput;
    private InputAction lookAction;
    private InputAction cursorAction;

    // Приватные переменные
    private Vector3 velocity = Vector3.zero;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private float currentDistance;
    private bool isCursorLocked = true;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;

    void Start()
    {
        currentDistance = -offset.z;
        InitializeCamera();
        SetupInputActions();
        FindPlayer();

        if (target != null)
        {
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
        }
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
        lookAction = Inputs.Instance.Controls.Shards.Look;
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
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }
    }

    void FixedUpdate()
    {
        UpdateCameraPosition();
    }

    void HandleCameraRotation()
    {
        if (!target || !isCursorLocked) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        //Debug.Log(target.GetComponentInParent<PlayerInput>().currentControlScheme);
        if (target.GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad")
        {
            float mouseX = lookInput.x * rotationSpeed * gamepadMultiplier * 0.1f;
            float mouseY = lookInput.y * rotationSpeed * gamepadMultiplier * 0.1f * (invertY ? 1 : -1);
            currentRotationY += mouseX;
            currentRotationX += mouseY;
        } else
        {
            float mouseX = lookInput.x * rotationSpeed * 0.1f;
            float mouseY = lookInput.y * rotationSpeed * 0.1f * (invertY ? 1 : -1);
            currentRotationY += mouseX;
            currentRotationX += mouseY;
        }

        currentRotationX = Mathf.Clamp(currentRotationX, -verticalAngleLimit, verticalAngleLimit);
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -currentDistance);

        Vector3 targetLookAt = target.position + Vector3.up * offset.y;
        Vector3 adjustedPosition = CheckCameraCollision(desiredPosition, targetLookAt);

        if (!IsTargetVisible(adjustedPosition, targetLookAt))
        {
            adjustedPosition = FindAlternativeCameraPosition(desiredPosition, targetLookAt);

            if (!IsTargetVisible(adjustedPosition, targetLookAt))
            {
                adjustedPosition = GetEmergencyPosition(targetLookAt);
            }
        }

        if (IsTargetVisible(adjustedPosition, targetLookAt))
        {
            lastValidPosition = adjustedPosition;
            lastValidRotation = Quaternion.LookRotation(targetLookAt - adjustedPosition);
        }

        transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref velocity, smoothSpeed);

        Vector3 lookDirection = targetLookAt - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * 2f);
        }

        if (debugVisualization)
        {
            Debug.DrawLine(targetLookAt, adjustedPosition, Color.green);
            Debug.DrawRay(targetLookAt, Vector3.up * 0.5f, Color.yellow);
        }
    }

    Vector3 CheckCameraCollision(Vector3 desiredPosition, Vector3 targetPos)
    {
        Vector3 direction = desiredPosition - targetPos;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(
            targetPos,
            cameraRadius,
            direction.normalized,
            out hit,
            distance,
            collisionMask))
        {
            float safeDistance = Mathf.Max(hit.distance - collisionOffset, minWallDistance);
            Vector3 safePosition = targetPos + direction.normalized * safeDistance;

            Vector3 fromObstacleToTarget = targetPos - hit.point;
            if (fromObstacleToTarget.magnitude < minWallDistance * 2f)
            {
                Vector3 forwardFromTarget = target.forward * emergencyForwardOffset;
                safePosition = targetPos + forwardFromTarget;
            }

            return safePosition;
        }

        return desiredPosition;
    }

    bool IsTargetVisible(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(
            fromPosition,
            cameraRadius * 0.5f,
            direction.normalized,
            out hit,
            distance - 0.1f,
            collisionMask))
        {
            if (hit.transform == target || hit.transform.IsChildOf(target))
                return true;

            return false;
        }

        return true;
    }

    Vector3 FindAlternativeCameraPosition(Vector3 originalPosition, Vector3 targetPos)
    {
        Vector3 bestPosition = originalPosition;
        float bestScore = 0f;

        Vector3 direction = originalPosition - targetPos;
        float currentDistance = direction.magnitude;

        for (int i = 0; i < 3; i++)
        {
            float testDistance = currentDistance * (1f - (i * 0.3f));
            Vector3 testPosition = targetPos + direction.normalized * testDistance;

            if (IsTargetVisible(testPosition, targetPos))
            {
                float distanceScore = 1f - (i * 0.2f);
                float visibilityScore = 1f;

                if (distanceScore + visibilityScore > bestScore)
                {
                    bestScore = distanceScore + visibilityScore;
                    bestPosition = testPosition;
                }
            }
        }

        if (!IsTargetVisible(bestPosition, targetPos) && enableFallbackPositions)
        {
            for (int i = 0; i < fallbackAngles.Length; i++)
            {
                float testAngle = currentRotationY + fallbackAngles[i];
                Quaternion testRotation = Quaternion.Euler(currentRotationX, testAngle, 0);
                Vector3 testPosition = targetPos + testRotation * Vector3.forward * -currentDistance * fallbackDistanceMultiplier;

                Vector3 adjustedPosition = CheckCameraCollision(testPosition, targetPos);

                if (IsTargetVisible(adjustedPosition, targetPos))
                {
                    return adjustedPosition;
                }
            }
        }

        return bestPosition;
    }

    Vector3 GetEmergencyPosition(Vector3 targetPos)
    {
        Vector3 forwardPosition = targetPos + target.forward * minDistance;

        RaycastHit hit;
        if (Physics.SphereCast(
            targetPos,
            cameraRadius,
            target.forward,
            out hit,
            minDistance * 2f,
            collisionMask))
        {
            Vector3 upwardPosition = targetPos + Vector3.up * minDistance;
            if (!Physics.CheckSphere(upwardPosition, cameraRadius, collisionMask))
            {
                return upwardPosition;
            }

            return lastValidPosition;
        }

        return forwardPosition;
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

    void OnDrawGizmosSelected()
    {
        if (!debugVisualization || target == null) return;

        Gizmos.color = Color.blue;
        Vector3 targetPos = target.position + Vector3.up * offset.y;
        Gizmos.DrawWireSphere(targetPos, 0.2f);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, cameraRadius);
            Gizmos.DrawLine(targetPos, transform.position);
        }
    }
}
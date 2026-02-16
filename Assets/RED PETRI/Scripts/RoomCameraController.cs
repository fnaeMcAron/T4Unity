using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance;

    public Transform player;

    [Header("Follow")]
    public float followLerp = 8f;
    public float snapSpeed = 40f;

    Bounds roomBounds;
    bool hasRoom;

    Camera cam;
    float camHalfW;
    float camHalfH;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();

        camHalfH = cam.orthographicSize;
        camHalfW = cam.aspect * camHalfH;
    }

    public void SetRoom(Bounds bounds)
    {
        roomBounds = bounds;
        hasRoom = true;

        Vector3 c = bounds.center;
        transform.position = new Vector3(c.x, c.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (!hasRoom || player == null) return;

        Vector3 target = player.position;

        float minX = roomBounds.min.x + camHalfW;
        float maxX = roomBounds.max.x - camHalfW;
        float minY = roomBounds.min.y + camHalfH;
        float maxY = roomBounds.max.y - camHalfH;

        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.y = Mathf.Clamp(target.y, minY, maxY);
        target.z = transform.position.z;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            followLerp * Time.deltaTime
        );
    }
}

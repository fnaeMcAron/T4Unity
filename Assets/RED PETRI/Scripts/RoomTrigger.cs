using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    BoxCollider2D col;
    public bool space;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RoomCameraController.Instance.SetRoom(col.bounds);

        var move = other.GetComponent<StudentMovement>();
        if (move != null)
            move.SetRoomSpace(space);
    }
}

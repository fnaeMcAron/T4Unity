using UnityEngine;

public class PhysButton : MonoBehaviour
{
    public GameObject door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StudentMovement player = collision.GetComponent<StudentMovement>();
        if (player == null) return;

        door.gameObject.SetActive(false);
    }
}

using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StudentMovement player = collision.GetComponent<StudentMovement>();
        if (player == null) return;

        player.Die();
    }
}

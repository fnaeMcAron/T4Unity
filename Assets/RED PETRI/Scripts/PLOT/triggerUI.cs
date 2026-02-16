using UnityEngine;
using UnityEngine.SceneManagement;

public class triggerUI : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StudentMovement player = collision.GetComponent<StudentMovement>();
        if (player == null) return;

        SceneManager.LoadScene("Ending");
    }
}

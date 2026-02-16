using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpeningUI : MonoBehaviour
{
    public GameObject[] panels;
    [SerializeField] int index = 0;

    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(i == 0);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        panels[index].SetActive(false);
        index++;

        if (index >= panels.Length)
        {
            SceneManager.LoadScene("Game");
            return;
        }

        panels[index].SetActive(true);
    }
}

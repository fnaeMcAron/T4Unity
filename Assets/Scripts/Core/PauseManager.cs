using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Gamepad.current?.startButton.wasPressedThisFrame == true)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        //Debug.Log("Игра на паузе");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // PauseUI.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        //Debug.Log("Игра продолжается");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // PauseUI.SetActive(false);
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }
}
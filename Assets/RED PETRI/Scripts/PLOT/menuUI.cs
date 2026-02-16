using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class menuUI : MonoBehaviour
{
    public GameObject buttons;
    public GameObject titles;

    bool isInputBlocked = false;
    InputAction anyInputAction;

    void Awake()
    {
        anyInputAction = new InputAction(binding: "*/<button>");
        anyInputAction.performed += OnAnyInput;
        anyInputAction.Enable();
        InputActionAsset layout = FindAnyObjectByType<InputActionAsset>();
    }

    void OnAnyInput(InputAction.CallbackContext context)
    {
        if (!buttons.activeSelf)
        {
            buttons.SetActive(true);
            titles.GetComponent<Animation>()["Titles"].time = -100f;
            titles.GetComponent<Animation>()["Titles"].normalizedTime = -100f;
            StartCoroutine(BlockInputCoroutine(0.5f));
        }
    }

    IEnumerator BlockInputCoroutine(float duration)
    {
        isInputBlocked = true;
        yield return new WaitForSecondsRealtime(duration);
        isInputBlocked = false;
    }

    public void Play()
    {
        SceneManager.LoadSceneAsync("Start");
    }

    public void Titles()
    {
        if (isInputBlocked)
            return;

        buttons.SetActive(false);
        titles.GetComponent<Animation>().Stop();
        titles.GetComponent<Animation>()["Titles"].time = 0f;
        titles.GetComponent<Animation>()["Titles"].normalizedTime = 0f;
        titles.GetComponent<Animation>().Play();
    }

    public void Exit()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class DocController : MonoBehaviour
{
    public GameObject menuUI;
    public PlayerInput playerInput;
    public StudentMovement player;

    bool menuOpen;

    public void OnOpenMenu(InputAction.CallbackContext ctx)
    {
        if (player.isDead) return;

        if (!ctx.performed) return;

        menuOpen = !menuOpen;

        if (menuOpen)
        {
            menuUI.SetActive(true);
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {

        menuUI.SetActive(false);
        playerInput.SwitchCurrentActionMap("Student");
        menuOpen = false;
        //Debug.Log(" ÕŒœ ¿ ∆»¬¿");
    }
}

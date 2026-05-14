using UnityEngine;
using UnityEngine.InputSystem;

public interface IGameState
{
    public void Enter() { }
    public void Exit() { }
    public void Update() { }
}

public class ShardsState : IGameState, Controls.IShardsActions
{
    private MainGameOrganizer context;
    public ShardsState(MainGameOrganizer ctx) => context = ctx;
    CharacterManager Charman =>
        (DungeonMaster.Instance.currentSceneContext as MainGameOrganizer)?.charman;

    public void Enter()
    {
        // 1. Говорим системе ввода: "Вызывай методы этого класса"
        Inputs.Instance.Controls.Shards.SetCallbacks(this);
        // 2. Включаем карту Shards
        Inputs.Instance.Controls.Shards.Enable();
        DungeonMaster.Instance.currentSceneContext.cam.GetComponent<CameraFollow>().SetTarget((DungeonMaster.Instance.currentSceneContext as MainGameOrganizer)?.charman.currentCharacter.gameObject.transform);
    }

    public void Exit()
    {
        // Очищаем колбэки при выходе и выключаем карту
        Inputs.Instance.Controls.Shards.SetCallbacks(null);
        Inputs.Instance.Controls.Shards.Disable();
    }

    public void Update()
    {

    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.canceled)
            DungeonMaster.Instance.TogglePause();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        /*var movement = (DungeonMaster.Instance.currentSceneContext as MainGameOrganizer)?.charman;
        if (movement == null) return;

        Vector2 input = context.ReadValue<Vector2>();
        Camera cam = DungeonMaster.Instance.currentSceneContext.cam;

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 worldDir = (forward * input.y + right * input.x);
        movement.SetMoveDirection(worldDir);*/
        Charman?.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        (DungeonMaster.Instance.currentSceneContext as MainGameOrganizer)?.charman.OnJump(context);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnCycleTarget(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnSwitchCharacter0(InputAction.CallbackContext context)
    {
        if (context.started) Charman?.SwitchToCharacter(0);
    }

    public void OnSwitchCharacter1(InputAction.CallbackContext context)
    {
        if (context.started) Charman?.SwitchToCharacter(1);
    }

    public void OnSwitchCharacter2(InputAction.CallbackContext context)
    {
        if (context.started) Charman?.SwitchToCharacter(2);
    }

    public void OnSwitchCharacter3(InputAction.CallbackContext context)
    {
        if (context.started) Charman?.SwitchToCharacter(3);
    }

    public void OnSwitchCharacter4(InputAction.CallbackContext context)
    {
        if (context.started) Charman?.SwitchToCharacter(4);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Charman?.OnAttack(context);
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        Charman?.OnDodge(context);
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        Charman?.OnAbility(context);
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        /*Debug.Log("не добавлено");
        if (context.started)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue != Vector2.zero)
            {
                charman.currentCharacter.SwitchWeapon(scrollValue);
            }
        }*/
        Charman?.OnSwitchWeapon(context);
    }

    public void OnResetCameraANDToggleTargetLock(InputAction.CallbackContext context)
    {
        Charman?.OnCameraAction(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        /*Debug.Log(context);
        DungeonMaster.Instance.gatekeeper.LoadLevel("Menu");*/
        Charman?.OnInteract(context);
    }

    public void OnD_MainMenu(InputAction.CallbackContext context)
    {
        /*
        Debug.Log(context);
        if (context.canceled)
            if (DungeonMaster.Instance.currentState is not sub_TerminalState)
                DungeonMaster.Instance.PushState(new sub_TerminalState());*/
        //Debug.Log(context);
        //DungeonMaster.Instance.gatekeeper.LoadLevel("Menu");
    }
}

public class sub_PauseState : IGameState, Controls.IPauseActions
{

    public void Enter()
    {
        Time.timeScale = 0f;
        Inputs.Instance.Controls.Pause.SetCallbacks(this);
        Inputs.Instance.Controls.Pause.Enable();
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        Inputs.Instance.Controls.Pause.SetCallbacks(null);
        Inputs.Instance.Controls.Pause.Disable();
    }

    public void Update()
    {

    }

    public void OnD_MainMenu(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        DungeonMaster.Instance.gatekeeper.LoadLevel("Menu");
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.canceled)
            DungeonMaster.Instance.TogglePause();
    }
}

public class sub_TerminalState : IGameState, Controls.ITerminalActions
{
    Terminal terminal;
    OrganizerBase previousContext;
    StudentMovement stdmove;
    DocController docmenu;

    public sub_TerminalState(Terminal ctx) => terminal = ctx;

    public void Enter()
    {
        Inputs.Instance.Controls.Terminal.SetCallbacks(this);
        Inputs.Instance.Controls.Terminal.Enable();
        DungeonMaster.Instance.currentSceneContext.cam.GetComponent<CameraFollow>().SetTarget(terminal.gameObject.transform);
        DungeonMaster.Instance.currentSceneContext.cam.GetComponent<CameraFollow>().ResetCameraBehindTarget();
        previousContext = DungeonMaster.Instance.currentSceneContext;

        if (DungeonMaster.Instance.currentSceneContext is IPlayerControllable controllable)
        {
            controllable.StopPlayer();
            
        }

        DungeonMaster.Instance.gatekeeper.LoadLevelAdditive("T_Game");
    }

    public void Exit()
    {
        Inputs.Instance.Controls.Terminal.SetCallbacks(null);
        Inputs.Instance.Controls.Terminal.Disable();

        DungeonMaster.Instance.currentSceneContext = previousContext;

        if (DungeonMaster.Instance.currentSceneContext is IPlayerControllable controllable)
        {
            controllable.ResumePlayer();
        }

        DungeonMaster.Instance.gatekeeper.UnloadLevel("T_Game");
        stdmove = null;
        docmenu = null;
    }

    public void Update()
    {
        if (stdmove == null || docmenu == null)
        {
            stdmove = (DungeonMaster.Instance.currentSceneContext as SubGameOrganizer)?.stdmove;
            docmenu = (DungeonMaster.Instance.currentSceneContext as SubGameOrganizer)?.docmenu;
        }
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        stdmove.OnDash(context);
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.canceled)
            DungeonMaster.Instance.PopState();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        stdmove.OnJump(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        stdmove.OnMove(context);
    }

    public void OnOpenDoc(InputAction.CallbackContext context)
    {
        docmenu.OnOpenMenu(context);
        Debug.Log("не добавлено");
    }

    public void OnSuicide(InputAction.CallbackContext context)
    {
        stdmove.Die();
    }
}

public class sub_CutsceneState : IGameState, Controls.ICutsceneActions
{
    public void Enter()
    {
        // 1. Говорим системе ввода: "Вызывай методы этого класса"
        Inputs.Instance.Controls.Cutscene.SetCallbacks(this);
        // 2. Включаем карту Pause
        Inputs.Instance.Controls.Cutscene.Enable();
    }

    public void Exit()
    {
        // Очищаем колбэки при выходе и выключаем карту
        Inputs.Instance.Controls.Cutscene.SetCallbacks(null);
        Inputs.Instance.Controls.Cutscene.Disable();
    }

    public void Update()
    {

    }

    public void OnHide(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnRead(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }
}

public class MainMenuState : IGameState, Controls.IUIActions
{
    private MainMenuOrganizer context;
    public MainMenuState(MainMenuOrganizer ctx) => context = ctx;

    public void Enter()
    {
        // 1. Говорим системе ввода: "Вызывай методы этого класса"
        Inputs.Instance.Controls.UI.SetCallbacks(this);
        // 2. Включаем карту Shards
        Inputs.Instance.Controls.UI.Enable();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Exit()
    {
        // Очищаем колбэки при выходе и выключаем карту
        Inputs.Instance.Controls.UI.SetCallbacks(null);
        Inputs.Instance.Controls.UI.Disable();
    }

    public void Update()
    {

    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        DungeonMaster.Instance.gatekeeper.LoadLevel("Game");
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        Debug.Log("не добавлено");
    }
}

// rts, shooter, race, horror, stealth, puzzle
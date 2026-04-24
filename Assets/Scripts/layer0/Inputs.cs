using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Inputs Instance { get; private set; }
    public Controls Controls { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Controls = new Controls();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //debug
    /*
    public void OnDebugSubstates(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if (context.canceled)
            if (DungeonMaster.Instance.currentState == DungeonMaster.Instance.cutsceneState)
                DungeonMaster.Instance.PopState();
            else
            {
                DungeonMaster.Instance.PushState(new sub_PauseState());
                DungeonMaster.Instance.PushState(DungeonMaster.Instance.cutsceneState);
            }
    }*/

    public void OnDebugEscape(InputAction.CallbackContext context)
    {
        if (context.canceled)
            DungeonMaster.Instance.PopState();
    }
}

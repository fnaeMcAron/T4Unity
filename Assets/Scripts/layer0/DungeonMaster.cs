using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DungeonMaster : MonoBehaviour
{
    public static DungeonMaster Instance { get; private set; }

    [Header("Текущие ссылки и состояния")]
    public Gatekeeper gatekeeper;
    public IGameState currentState { get; private set; }
    public OrganizerBase currentSceneContext;
    public sub_PauseState PauseState;

    [Header("DEBUG")]
    public TMP_Text text;

    private Stack<IGameState> stateStack = new Stack<IGameState>();

    public void SwitchState(IGameState newState)
    {
        currentState?.Exit();
        stateStack.Clear();
        currentState = newState;
        currentState?.Enter();
    }

    public void PushState(IGameState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
            stateStack.Push(currentState);
        }
        currentState = newState;
        currentState.Enter();
    }

    public void PopState()
    {
        currentState?.Exit();
        if (stateStack.Count > 0)
        {
            currentState = stateStack.Pop();
            currentState.Enter();
        }
        else
        {
            // Если стек пуст, возвращаемся к начальному стейту сцены
            if (currentSceneContext != null) SwitchState(currentSceneContext.initialState);
        }
    }

    void Awake()
    {
        PauseState = new sub_PauseState();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        //Application.targetFrameRate = 8000;
    }

    void Update()
    {
        currentState?.Update();
        text.text = $"{currentState}";
        Debug.Log(text.text);
    }

    public void TogglePause()
    {
        if (currentState is sub_PauseState) 
            PopState();
        else 
            PushState(DungeonMaster.Instance.PauseState);
    }

    public void RegisterScene(OrganizerBase context)
    {
        currentSceneContext = context;

        Debug.Log("Контекст сцены принят");
        SwitchState(context.initialState);
    }

    public void RegisterSubscene(OrganizerBase context)
    {
        currentSceneContext = context;

        Debug.Log("Контекст сабсцены принят");
    }
}

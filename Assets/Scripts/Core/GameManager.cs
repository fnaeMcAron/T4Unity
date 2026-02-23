using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameStateBase currentState;
    public Layout inputActions;
    public bool temp;

    public ShardsGS shardsState = new ShardsGS();
    public TerminalGS terminalState = new TerminalGS();
    //public PauseGS pauseState = new PauseGS();

    public static GameManager Instance { get; private set; }

    private void Start()
    {
        currentState = shardsState;
        currentState.Enter(this);
    }

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

        InitializeGame();
    }

    void InitializeGame()
    {
        // TO DO: всякие начальные действия
    }

    private void Update()
    {
        currentState.Update(this);
    }

    public void SwitchState(GameStateBase nextState)
    {
        currentState.Exit(this);
        currentState = nextState;
        currentState.Enter(this);
    }
}
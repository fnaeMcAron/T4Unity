using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GameStateBase
{
    public virtual void Enter(GameManager gameman) { }
    public virtual void Update(GameManager gameman) { }
    public virtual void Exit(GameManager gameman) { }
}

public interface IManager
{

}

public class ShardsGS : GameStateBase
{
    public override void Enter(GameManager gameman)
    {
        gameman.gameObject.GetComponent<PlayerInput>().currentActionMap = gameman.inputActions.Shards;
        CharacterManager.Instance.enabled = true;
    }

    public override void Update(GameManager gameman)
    {
        if (CharacterManager.Instance.closestInteractable != null)
        {
            if (CharacterManager.Instance.closestInteractable.GetComponentInChildren<Termanal>() != null)
            {
                Termanal ter = CharacterManager.Instance.closestInteractable.GetComponentInChildren<Termanal>();
                if (ter != null & ter.wasInteracted)
                {
                    Debug.LogAssertion("терминал");
                    // добавить ter.wasInteracted = false; после выхода из терминала
                }
            }
        }
    }

    public override void Exit(GameManager gameman)
    {
        CharacterManager.Instance.enabled = false;
    }
}

public class TerminalGS : GameStateBase
{
    public override void Enter(GameManager gameman)
    {

    }

    public override void Update(GameManager gameman)
    {

    }

    public override void Exit(GameManager gameman)
    {

    }
}

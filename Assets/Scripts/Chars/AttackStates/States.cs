using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase
{
    public virtual void Enter(CharacterManager charman) { }
    public virtual void Update(CharacterManager charman) { }
    public virtual void Exit(CharacterManager charman) { }
}

//стоя
public class IdleState : StateBase
{
    public override void Enter(CharacterManager charman)
    {

    }

    public override void Update(CharacterManager charman)
    {
        if (!charman.currentCharacter.isGrounded)
        {
            charman.SwitchState(charman.midairState);
        }
    }

    public override void Exit(CharacterManager charman)
    {
        
    }
}

//прыжок
public class MidairState : StateBase
{
    public override void Enter(CharacterManager charman)
    {
        charman.currentCharacter.Jump();
    }

    public override void Update(CharacterManager charman)
    {
        if (charman.currentCharacter.isGrounded)
        {
            charman.SwitchState(charman.idleState);
        }
    }

    public override void Exit(CharacterManager charman)
    {

    }
}

//применение способности
public class AbilityState : StateBase
{
    public AbilityState() { }

    public override void Enter(CharacterManager charman)
    {
        charman.currentCharacter.UseAbility(false);
        charman.SwitchState(charman.idleState);
    }

    public override void Update(CharacterManager charman) { }

    public override void Exit(CharacterManager charman) { }
}

//атака
public class AttackState : StateBase
{
    public AttackState(bool midair) { }

    public override void Enter(CharacterManager charman)
    {

    }

    public override void Update(CharacterManager charman)
    {

    }

    public override void Exit(CharacterManager charman)
    {

    }
}

//уворот
public class DodgeState : StateBase
{
    public DodgeState(bool midair) { }

    public override void Enter(CharacterManager charman)
    {
        if (!charman.currentCharacter.isGrounded)
        {
            charman.currentCharacter.Dodge();
            charman.SwitchState(charman.midairState);
        }
        else
        {
            charman.currentCharacter.Dodge();
            charman.SwitchState(charman.idleState);
        }
    }

    public override void Update(CharacterManager charman)
    {
        charman.currentCharacter.Dodge();
    }

    public override void Exit(CharacterManager charman)
    {

    }
}

//зажатые

//зажатая атака
public class HoldenAttackState : StateBase
{
    public HoldenAttackState(bool midair) { }

    public override void Enter(CharacterManager charman)
    {

    }

    public override void Update(CharacterManager charman)
    {

    }

    public override void Exit(CharacterManager charman)
    {

    }
}

//езда
public class HoldenDodgeState : StateBase
{
    public HoldenDodgeState() { }

    public override void Enter(CharacterManager charman)
    {
        if (!charman.currentCharacter.isGrounded)
        {
            Debug.Log("ю шулд граунд юрселф. НАУ.");
            charman.SwitchState(charman.midairState);
        }
        else
        {
            // todo
            //Riding();
            charman.SwitchState(charman.idleState);
        }
    }

    public override void Update(CharacterManager charman)
    {
        // todo
        //Riding();
    }

    public override void Exit(CharacterManager charman) { }
}

//зажатая способность
public class HoldenAbilityState : StateBase
{
    public HoldenAbilityState() { }

    public override void Enter(CharacterManager charman)
    {
        charman.currentCharacter.UseAbility(true);
        charman.SwitchState(charman.idleState);
    }

    public override void Update(CharacterManager charman) { }

    public override void Exit(CharacterManager charman) { }
}
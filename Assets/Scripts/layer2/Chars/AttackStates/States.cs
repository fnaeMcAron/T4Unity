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
    public AttackState() { }

    public override void Enter(CharacterManager charman)
    {
        if (Time.time - charman.lastAttackTime > charman.currentCharacter.comboTimeWindow)
        {
            charman.unchargedAttackCount = 0;
        }

        charman.unchargedAttackCount++;
        charman.lastAttackTime = Time.time;

        if (charman.unchargedAttackCount > charman.currentCharacter.maxComboCount)
        {
            charman.unchargedAttackCount = 0;
        }

        Debug.Log($"Комбо: {charman.unchargedAttackCount} незаряженных атак");

        if (charman.currentCharacter.currentWeaponIndex == 0)
        {
            // Генерируем случайную атаку от 1 до 2
            int attackIndex = Random.Range(1, 3);

            // Запускаем соответствующую анимацию атаки
            if (attackIndex == 1)
            {
                charman.currentCharacter.animator.SetTrigger(charman.currentCharacter.firstMeleeAttackTriggerHash);
            }
            else if (attackIndex == 2)
            {
                charman.currentCharacter.animator.SetTrigger(charman.currentCharacter.secondMeleeAttackTriggerHash);
            }

            charman.currentCharacter.PerformMeleeAttack();
        }
        else if (charman.currentCharacter.currentWeaponIndex == 1)
        {
            charman.currentCharacter.animator.SetTrigger(charman.currentCharacter.rangedAttackHash);
            charman.currentCharacter.PerformRangedAttack();
        }

        charman.lastAttackTime = Time.time;
    }

    public override void Update(CharacterManager charman)
    {
        if (charman.currentCharacter.moveInput != Vector2.zero)
        {
            if (charman.currentCharacter.isGrounded)
            {
                charman.SwitchState(charman.idleState);
            }
            else
            {
                charman.SwitchState(charman.midairState);
            }
        }
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
    public HoldenAttackState() { }

    public override void Enter(CharacterManager charman)
    {
        if (charman.currentCharacter.currentWeaponIndex == 0)
        {
            charman.currentCharacter.PerformMeleeChargeAttack();
        }
        else if (charman.currentCharacter.currentWeaponIndex == 1)
        {
            charman.currentCharacter.PerformRangedAim();
        }
    }

    public override void Update(CharacterManager charman)
    {
        if (charman.currentCharacter.moveInput != Vector2.zero)
        {
            if (charman.currentCharacter.isGrounded)
            {
                charman.SwitchState(charman.idleState);
            }
            else
            {
                charman.SwitchState(charman.midairState);
            }
        }
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
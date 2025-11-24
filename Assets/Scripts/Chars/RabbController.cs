using UnityEngine;

public class RabbController : CharacterBase
{
    [Header("Настройки Крола")]
    public WeaponRaycast raycast;

    public override void PerformMeleeAttack(bool isHold)
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Крол: атака тростью в ближнем бою");
    }

    public override void PerformRangedAttack(bool isHold)
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Крол: выстрелы томпсоном в дальнем бою");
        ShootTompson();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Крол: миньоны");
        //TO DO: как сделать систему миньонов?
    }

    public override void Dodge()
    {
        Debug.Log("Крол: уворот");
        // TO DO: уворот
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }




    void ShootTompson()
    {
        if (raycast != null)
        {
            raycast.Shoot();
        }
        else
        {
            Debug.LogWarning("WeaponRaycast не назначен для Еки");
        }
    }
}
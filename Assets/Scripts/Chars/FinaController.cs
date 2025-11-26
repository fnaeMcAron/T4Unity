using UnityEngine;

public class FinaControler : CharacterBase
{
    //[Header("Настройки Фины")]

    public override void PerformMeleeAttack()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Фина: атака палкой в ближнем бою");
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Фина: атака копьем в дальнем бою");
        ShootSpearProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Фина: стан");
        StunInSphere();
    }

    protected override void Dodge()
    {
        Debug.Log("Фина: уворот");
        // TO DO: уворот
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }



    void ShootSpearProjectile()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward;
        projectile.GetComponent<Renderer>().material.color = Color.red;
        // TO DO: добавить Rigidbody и логику полета



        /*
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = transform.position + transform.forward;
        bullet.transform.localScale = Vector3.one * 0.2f;
        bullet.GetComponent<Renderer>().material.color = Color.blue;

        Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
        bulletRb.useGravity = false;
        bulletRb.velocity = transform.forward * 25f;

        // Добавляем коллайдер и тег для идентификации
        bullet.tag = "PlayerProjectile";
        Destroy(bullet, 2f);
        */
    }

    void StunInSphere()
    {

    }
}
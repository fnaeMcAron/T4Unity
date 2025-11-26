using UnityEngine;

public class FnaeController : CharacterBase
{
    [Header("Настройки Фная")]
    public float burnDamage = 10f;
    public GameObject fireEffect;

    public override void PerformMeleeAttack()
    {
        Debug.Log($"Фнай: атака {damageMultiplier}");
        StartCoroutine(EnablingCollider(1f, 0));
        //CreateBurnEffect();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log($"Фнай: атака {damageMultiplier}");
        weaponSlots[1].weaponObject.GetComponent<Collider>().enabled = true;
        ShootFireProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log($"Фнай: молотов {damageMultiplier}");
        IgniteArea();
    }

    protected override void Dodge()
    {
        Debug.Log("Фнай: уворот на тиранозавре");
    }

    public override void PerformMeleeChargeAttack()
    {
        weaponSlots[0].weaponObject.GetComponent<Collider>().enabled = true;
    }

    public override void PerformRangedAim()
    {

    }

    private void CreateBurnEffect()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        if (fireEffect != null)
        {
            Instantiate(fireEffect, transform.position, Quaternion.identity);
        }
    }

    private void ShootFireProjectile()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward;
        projectile.GetComponent<Renderer>().material.color = Color.red;
        // TO DO: добавить Rigidbody и логику полета
    }

    private void IgniteArea()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // TO DO: нанести урон горением
            }
        }
    }
}
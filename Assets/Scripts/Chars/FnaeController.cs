using UnityEditor;
using UnityEngine;

public class FnaeController : CharacterBase
{
    [Header("Настройки Фная")]
    public float burnDamage = 10f;
    public GameObject molotovPrefab;
    //public GameObject fireEffect;

    public override void PerformMeleeAttack()
    {
        Debug.Log($"Фнай: атака {damageMultiplier}");
        StartCoroutine(EnablingCollider(1f, 0, 10, "Кулаками"));
        //CreateBurnEffect();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log($"Фнай: атака {damageMultiplier}");
        ShootFireProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log($"Фнай: молотов {damageMultiplier}");
        ShootFireProjectile();
    }

    public override void Dodge()
    {
        Debug.Log("Фнай: уворот на тиранозавре");
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }

    /*private void CreateBurnEffect()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        if (fireEffect != null)
        {
            Instantiate(fireEffect, transform.position, Quaternion.identity);
        }
    }*/

    private void ShootFireProjectile()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации

        GameObject projectile = Instantiate(molotovPrefab, transform.position + transform.up * 2, Quaternion.identity);
        Rigidbody molotovPhys = projectile.GetComponent<Rigidbody>();

        projectile.GetComponent<Renderer>().material.color = Color.red;
        molotovPhys.velocity = new Vector3(cameraFollow.transform.forward.x * moveSpeed, 5f, cameraFollow.transform.forward.z * moveSpeed);
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
                enemy = hitCollider.GetComponent<Enemy>();
                enemy.TakeDamage(25f);
                StartCoroutine(enemy.Stun(10));
            }
        }
    }
}
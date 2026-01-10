using UnityEngine;

public class FinaController : CharacterBase
{

    [Header("Особые настройки")]
    public float lowHealthDamageBonus = 2.0f;
    public float humiliationThreshold = 0.3f;

    public override void OnCharacterSelected()
    {
        base.OnCharacterSelected();
        if (styleManager != null)
        {
            styleManager.SwitchToFinaStyle();
        }
    }

    public override void PerformMeleeAttack()
    {
        Debug.Log("Фина: атака палкой");
        // Получаем урон с учетом модификаторов стиля
        float baseDamage = weaponSlots[currentWeaponIndex].baseDamage;
        float styleMultiplier = styleManager?.GetCurrentDamageMultiplier() ?? 1f;

        // Дополнительный бонус при низком HP
        float healthPercent = GetHealthPercent();
        if (healthPercent < humiliationThreshold)
        {
            styleMultiplier *= lowHealthDamageBonus;
        }

        float finalDamage = baseDamage * styleMultiplier;
        Debug.Log($"Урон Фины: {finalDamage} (множитель: {styleMultiplier})");

        if (styleManager != null && styleManager.IsStyleActive())
        {
            // Для Фины плохая игра добавляет очки
            styleManager.AddStylePoints(10, "Атака");
        }
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Фина: атака копьем");
        ShootSpearProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Фина: стан");
        StunInSphere();

        if (styleManager != null && styleManager.IsStyleActive())
        {
            // Стоимость способности зависит от уровня стиля
            float costModifier = styleManager.GetCurrentResourceCostModifier();
            // TO DO: применить к стоимости червей
        }
    }

    private float GetHealthPercent()
    {
        // TO DO: получить процент HP из WormManager
        return 1.0f;
    }

    public override void Dodge()
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

        if (styleManager != null && styleManager.IsStyleActive())
        {
            styleManager.AddStylePoints(15, "Метание копья");
        }

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
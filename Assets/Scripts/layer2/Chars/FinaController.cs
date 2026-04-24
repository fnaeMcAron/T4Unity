using UnityEngine;

public class FinaController : CharacterBase
{

    [Header("������ ���������")]
    public float lowHealthDamageBonus = 2.0f;
    public float humiliationThreshold = 0.3f;
    public GameObject spear;
    public float spearSpeed;

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
        Debug.Log("����: ����� ������");
        // �������� ���� � ������ ������������� �����
        float baseDamage = weaponSlots[currentWeaponIndex].baseDamage;
        float styleMultiplier = styleManager?.GetCurrentDamageMultiplier() ?? 1f;

        // �������������� ����� ��� ������ HP
        float healthPercent = GetHealthPercent();
        if (healthPercent < humiliationThreshold)
        {
            styleMultiplier *= lowHealthDamageBonus;
        }

        float finalDamage = baseDamage * styleMultiplier;
        Debug.Log($"���� ����: {finalDamage} (���������: {styleMultiplier})");

        if (styleManager != null && styleManager.IsStyleActive())
        {
            // ��� ���� ������ ���� ��������� ����
            styleManager.AddStylePoints(10, "�����");
        }
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("����: ����� ������");
        ShootSpearProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("����: ����");
        StunInSphere();

        if (styleManager != null && styleManager.IsStyleActive())
        {
            // ��������� ����������� ������� �� ������ �����
            float costModifier = styleManager.GetCurrentResourceCostModifier();
            // TO DO: ��������� � ��������� ������
        }
    }

    private float GetHealthPercent()
    {
        // TO DO: �������� ������� HP �� WormManager
        return 1.0f;
    }

    public override void Dodge()
    {
        Debug.Log("����: ������");
        // TO DO: ������
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }



    void ShootSpearProjectile()
    {
        GameObject _projectile = Instantiate(spear, transform.position + Vector3.up * 4f, transform.rotation);
        Vector3 direction = cameraFollow.gameObject.transform.forward;
        _projectile.GetComponent<Rigidbody>().linearVelocity = direction.normalized * spearSpeed;
        /*
        // TODO: �������� ��������������� ����� � �������� ����������
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward;
        projectile.GetComponent<Renderer>().material.color = Color.red;
        // TO DO: �������� Rigidbody � ������ ������

        if (styleManager != null && styleManager.IsStyleActive())
        {
            styleManager.AddStylePoints(15, "������� �����");
        }
        */
        /*
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = transform.position + transform.forward;
        bullet.transform.localScale = Vector3.one * 0.2f;
        bullet.GetComponent<Renderer>().material.color = Color.blue;

        Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
        bulletRb.useGravity = false;
        bulletRb.velocity = transform.forward * 25f;

        // ��������� ��������� � ��� ��� �������������
        bullet.tag = "PlayerProjectile";
        Destroy(bullet, 2f);
        */
    }

    void StunInSphere()
    {
        // TODO: �������� ��������������� ����� � �������� ����������
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
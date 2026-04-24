using UnityEditor;
using UnityEngine;

public class FnaeController : CharacterBase
{
    [Header("��������� ����")]
    public float burnDamage = 10f;
    public GameObject molotovPrefab;
    //public GameObject fireEffect;

    public override void PerformMeleeAttack()
    {
        Debug.Log($"����: ����� {damageMultiplier}");
        StartCoroutine(EnablingCollider(1f, 0, 10, "��������"));
        //CreateBurnEffect();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log($"����: ����� {damageMultiplier}");
        ShootFireProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log($"����: ������� {damageMultiplier}");
        ShootFireProjectile();
    }

    public override void Dodge()
    {
        Debug.Log("����: ������ �� �����������");
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }

    /*private void CreateBurnEffect()
    {
        // TODO: �������� ��������������� ����� � �������� ����������
        if (fireEffect != null)
        {
            Instantiate(fireEffect, transform.position, Quaternion.identity);
        }
    }*/

    private void ShootFireProjectile()
    {
        // TODO: �������� ��������������� ����� � �������� ����������

        GameObject projectile = Instantiate(molotovPrefab, transform.position + transform.up * 2, Quaternion.identity);
        Rigidbody molotovPhys = projectile.GetComponent<Rigidbody>();

        projectile.GetComponent<Renderer>().material.color = Color.red;
        molotovPhys.linearVelocity = new Vector3(cameraFollow.transform.forward.x * moveSpeed, 5f, cameraFollow.transform.forward.z * moveSpeed);
        // TO DO: �������� Rigidbody � ������ ������
    }

    private void IgniteArea()
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
using UnityEngine;

public class MolotovProjectile : MonoBehaviour
{
    public float damagePerSecond = 10f;
    public float burnDuration = 5f;
    public float fireRadius = 3f;
    public GameObject fireEffectPrefab;

    private bool hasExploded = false;
    private Rigidbody rb;
    private SphereCollider triggerCollider;
    private GameObject fireEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = fireRadius;
        triggerCollider.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        Explode();
    }

    void Explode()
    {
        hasExploded = true;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        GameObject fireSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fireSphere.transform.position = transform.position;
        fireSphere.transform.localScale = Vector3.one * fireRadius * 2f;

        Renderer sphereRenderer = fireSphere.GetComponent<Renderer>();
        sphereRenderer.material.color = new Color(1f, 0.3f, 0f, 0.3f);

        Destroy(fireSphere.GetComponent<Collider>());

        triggerCollider.enabled = true;

        if (fireEffectPrefab != null)
        {
            fireEffect = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
            fireEffect.transform.localScale = Vector3.one * fireRadius;
        }

        Destroy(fireSphere, burnDuration);
        Destroy(gameObject, burnDuration);
        if (fireEffect != null)
        {
            Destroy(fireEffect, burnDuration);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!hasExploded)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, fireRadius);
        }
    }
}
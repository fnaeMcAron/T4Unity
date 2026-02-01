using UnityEngine;

public class WeaponPhys : MonoBehaviour
{
    public float damage;
    public enum Vars
    {
        isDestroy,
        isReturnable
    }
    public Vars thisVar;

    bool hitTarget;

    private void OnCollisionEnter(Collision collision)
    {
        if (!this.GetComponent<Collider>().isTrigger)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
            }
            else
            {
                if (thisVar == Vars.isDestroy)
                    Destroy(this.gameObject);
            }
        }
            
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (this.GetComponent<Collider>().isTrigger)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
            }
            else
            {
                if (thisVar == Vars.isDestroy)
                    Destroy(this.gameObject);
            }
        }
    }
}

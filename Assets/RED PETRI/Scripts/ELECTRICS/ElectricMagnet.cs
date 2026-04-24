using UnityEngine;

public class ElectricMagnet : MonoBehaviour, IElectricSensitive
{
    public float force = 10f;
    Rigidbody2D rb;
    Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnElectricEnter(GameObject source)
    {
        target = source.transform;
        Debug.Log("����");
    }

    public void OnElectricExit(GameObject source)
    {
        target = null;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("�����");
    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 dir = (target.position - transform.position).normalized;
        rb.AddForce(dir * force);
    }
}

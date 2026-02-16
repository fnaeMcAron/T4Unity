using UnityEngine;

public class ElectricEmitter : MonoBehaviour
{
    [SerializeField] Collider2D area;
    public StudentMovement movement;

    void Awake()
    {
        movement = GetComponent<StudentMovement>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!movement.canActivateElectric)
            return;

        if (!col.CompareTag("ElectricSensitive"))
            return;

        var sensitive = col.GetComponent<IElectricSensitive>();
        if (sensitive != null)
        {
            sensitive.OnElectricEnter(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!movement.canActivateElectric)
            return;

        if (!col.CompareTag("ElectricSensitive"))
            return;

        var sensitive = col.GetComponent<IElectricSensitive>();
        if (sensitive != null)
        {
            sensitive.OnElectricExit(gameObject);
        }
    }
}

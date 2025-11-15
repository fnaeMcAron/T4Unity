using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    GameObject[] player;
    public float health = 100f;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider collider;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        //agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        agent.SetDestination(player[0].transform.position);
        agent.autoRepath = true;
        agent.autoBraking = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    public void Stun()
    {

    }
}
using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    GameObject[] player;
    public float health = 100f;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider enemyCollider;
    CharacterManager _charman;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("GameController");
        agent = GetComponent<NavMeshAgent>();
        _charman = player[0].gameObject.GetComponent<CharacterManager>();
    }

    public void Update()
    {
        agent.SetDestination(_charman.CurrentCharacter.transform.position);
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

    public void DealDamage()
    {

    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    public void Stun()
    {

    }
}
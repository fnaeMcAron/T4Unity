using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    GameObject[] player;
    public float health = 100f;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    public float wormReward;
    //[SerializeField] Collider enemyCollider;
    CharacterManager _charman;
    StyleManager _styleman;
    CharacterBase _char;
    float incomingDamage;
    bool isStunned;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("GameController");
        agent = GetComponent<NavMeshAgent>();
        _styleman = player[0].gameObject.GetComponent<StyleManager>();
        _charman = player[0].gameObject.GetComponent<CharacterManager>();
    }

    public void Update()
    {
        
        if (isStunned)
        {

        }
        else
        {
            agent.SetDestination(_charman.CurrentCharacter.transform.position);
        }
        agent.autoRepath = true;
        agent.autoBraking = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            _char = _charman.CurrentCharacter.GetComponent<CharacterBase>();
            incomingDamage = _char.weaponSlots[_char.currentWeaponIndex].baseDamage * _char.damageMultiplier * _styleman.GetCurrentDamageMultiplier();
            TakeDamage(incomingDamage);
            //todo вылетающие цифры?
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

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
        Destroy(gameObject);
    }

    public IEnumerator Stun(float seconds)
    {
        isStunned = true;
        agent.ResetPath();
        yield return new WaitForSeconds(seconds);
        isStunned = false;
    }
}
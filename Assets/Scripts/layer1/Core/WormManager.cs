using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    private float worms = 100f; //заменить на гет сет позже
    public float maxWorms = 100f;

    // Событие для обновления UI
    public System.Action<float> OnWormsChanged;

    public bool SpendWorms(float amount)
    {
        if (worms >= amount)
        {
            worms -= amount;
            OnWormsChanged?.Invoke(worms);
            return true;
        }
        return false;
    }

    public void GainWorms(float amount)
    {
        worms = Mathf.Min(worms + amount, maxWorms);
        OnWormsChanged?.Invoke(worms);
    }

    public void OnEnemyKilled(Enemy enemy)
    {
        GainWorms(enemy.wormReward);
    }

    public float GetWorms()
    {
        return worms;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
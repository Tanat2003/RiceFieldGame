using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currenthealth;
    protected virtual void Awake()
    {
        currenthealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage)
    {
        currenthealth -= damage;
    }
    public virtual void IncreaseHealth()
    {
        currenthealth ++;
        if(currenthealth > maxHealth)
        {
            currenthealth = maxHealth;
        }
    }
    public bool ShouldDie() => currenthealth <= 0;
}

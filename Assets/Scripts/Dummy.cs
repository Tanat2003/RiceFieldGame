using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour,IDamagable
{
    public int currentHealth;
    public int maxHealth = 100;
    [Header("Material")]
    public MeshRenderer mesh;
    public Material material;
    public Material deadMaterial;
    [Space]
    public float refreshCooldown;
    private float lastTimeDamage;

    private void Start()
    {
        Refresh();
    }
    private void Update()
    {
        if(Time.timeAsDouble > refreshCooldown+lastTimeDamage)
        {
            Refresh();
        }
    }
    private void Refresh()
    {
        currentHealth = maxHealth;
        mesh.sharedMaterial = material;

    }

    public void TakeDamage(int damage)
    {
        lastTimeDamage = Time.time;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        
        mesh.sharedMaterial = deadMaterial;
    }
}

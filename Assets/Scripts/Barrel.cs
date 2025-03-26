using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamagable
{
    public void TakeDamage(int damage)
    {
        Debug.Log("HitBarrel");
    }
}

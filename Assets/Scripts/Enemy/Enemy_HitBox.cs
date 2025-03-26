using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy_HitBox : HitBox
{
    private Enemy enemy;
    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
    }
    public override void TakeDamage(int damage)
    {
        int newDaamge = Mathf.RoundToInt(damage*damageMultiplier);
        
        enemy.GetHit(newDaamge);
    }
}

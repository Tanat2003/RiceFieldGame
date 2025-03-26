using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour,IDamagable
{
    //ศัตรูที่ถือโล่ต้องปิดEnableของโล่ด้วยบวกติ้กถูกIsKinemetic เพื่อป้องกันบัคRagdollหาRigibodyไม่เจอตอนโล่โดนทำลาย
    private Enemy_Melee enemy;
    [SerializeField] private int durability; //HPของโล่

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
        durability =enemy.shieldDurability;
    }
    public void ReduceDurability(int damage)
    {
        durability -= damage;
        
        if(durability <0)
        {
            //เปลี่ยนอนิเมชั่นวิ่งไล่เป็นวิ่งแบบไม่ถือโล่
            enemy.animator.SetFloat("ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}

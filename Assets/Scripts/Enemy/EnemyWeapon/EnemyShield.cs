using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour,IDamagable
{
    //�ѵ�ٷ��������ͧ�ԴEnable�ͧ�����ºǡ��顶١IsKinemetic ���ͻ�ͧ�ѹ�ѤRagdoll��Rigibody����͵͹���ⴹ�����
    private Enemy_Melee enemy;
    [SerializeField] private int durability; //HP�ͧ���

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
            //����¹͹�����������������Ẻ��������
            enemy.animator.SetFloat("ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}

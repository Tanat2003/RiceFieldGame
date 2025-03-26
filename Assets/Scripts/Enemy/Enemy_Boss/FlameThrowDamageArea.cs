using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowDamageArea : MonoBehaviour
{
    private EnemyBoss enemy;
    private float damageCoolDown;
    private float lastTimeDamage;
    private int flameDamage;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBoss>();
        damageCoolDown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
    }

    private void OnTriggerStay(Collider other) //�����obj�����Collider�ҹ���˹
    {
        if(enemy.flameThrowActive == false)
        {
            return;
        }

        
        if (Time.time - lastTimeDamage < damageCoolDown)
        {
            return;
        }

        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage);
            lastTimeDamage = Time.time; //Update ��������ش���Ӵ�����
            damageCoolDown = enemy.flameDamageCooldown; //�Ѿഷcooldowndamage�ء���駷���Ҽ�����
            
        }
        
    }


}

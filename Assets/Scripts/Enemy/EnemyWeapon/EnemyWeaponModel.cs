using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponModel : MonoBehaviour
{
    public EnemyMeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController; //ไว้ใช้สำหรับศัตรูที่ถืออาวุธแตกต่างกันไป
    public EnemyWeaponData weaponData;


    [SerializeField] private GameObject[] trailEffects;
    [Header("Damage atribute")]
    public Transform[] damagePoints;
    public float attackRadius;

    [ContextMenu("Assign damage Point tranfroms")]
    private void GetDamagePoints()
    {
        damagePoints = new Transform[trailEffects.Length];
        for(int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] =trailEffects[i].transform;

        }
    }
    private void Awake()
    {
        EnableTrailEffects(false);
    }
    public void EnableTrailEffects(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }
    private void OnDrawGizmos()
    {
        if(damagePoints.Length > 0)
        {
            foreach(Transform point in damagePoints)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(point.position,attackRadius);
            }
        }
    }
}

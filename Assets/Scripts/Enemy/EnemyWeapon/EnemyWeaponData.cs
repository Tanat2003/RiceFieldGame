using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy/WeaponData")]
public class EnemyWeaponData : ScriptableObject
{
    public List<MeleeAttackData> attackData;
    
}

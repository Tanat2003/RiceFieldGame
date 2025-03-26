using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data",menuName ="WeaponSystem/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Bullet")]
    public int bulletDamage;

    [Header("Spesifics")]
    public string weaponName;
    public WeaponType weaponType;
    [Range(1,5)]
    public float reloadSpeed=2;
    [Range(1, 5)]
    public float equipmentSpeed = 1;
    [Range(4,10)]
    public float gunDistance = 4;
    [Range(4, 13)]
    public float cameraDistance = 6;

    [Header("Magazine Detail")]
    public int ammoInMagazine; //กระสุนปัจจุบันที่เรามี 2/30
    public int magazineCapacity; //ขนาดของแม็กกะซีน
    public int totalReserveAmmo; //จำนวนกระสุนทั้งหมดของปืนนี้ที่เรามี

    [Header("ShootDetail")]
    public float fireRate;
    public ShootType shootType;
    public int bulletPerShot =1;

    [Header("Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = .25f;


    [Header("Burst")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletPerShot; //จำนวนกระสุนต่อ1การยิง
    public float burstFireRate; //1วิยิงได้กี่นัด
    public float burstFireDelay =.1f; //ยิงแต่ละครั้งให้ดีเลย์เท่าไหร่




}

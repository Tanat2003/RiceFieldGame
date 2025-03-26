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
    public int ammoInMagazine; //����ع�Ѩ�غѹ�������� 2/30
    public int magazineCapacity; //��Ҵ�ͧ��硡Ыչ
    public int totalReserveAmmo; //�ӹǹ����ع�������ͧ�׹����������

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
    public int burstBulletPerShot; //�ӹǹ����ع���1����ԧ
    public float burstFireRate; //1���ԧ����Ѵ
    public float burstFireDelay =.1f; //�ԧ���Ф������������������




}

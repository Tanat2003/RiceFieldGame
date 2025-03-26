using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_WeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f; //�������ǡ���ع��鹰ҹ�������ٵäӹǹ
    private Player player;

    [SerializeField] private WeaponData defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponIsReady;
    private bool isShooting;

    [Space]
    [Header("Bullet Detail")]
    [SerializeField] private float bulletImpactForce=100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [Space]

    [Header("ForDebuggingBulletDirection")]
    [SerializeField] private Transform weaponHolder;
    [Space]

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private GameObject weaponPickupPrefab; 




    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvent();

        currentWeapon.ammoInMagazine = currentWeapon.totalReserveAmmo;
        Invoke("EquipStartingWeapon", .1f); //invoke��͡�����¡��ѧ������մ�����
    }
    private void Update()
    {
        if (isShooting)
        {
            Shoot();
        }

    }


    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.bulletPerShot; i++)
        {
            FireSingleBullet();
            yield return new WaitForSeconds(currentWeapon.burst_FireDelay);
            if (i >= currentWeapon.bulletPerShot)
            {
                SetWeaponReady(true);
            }
        }
    }
    private void Shoot()
    {
        if (WeaponReady() == false)
        {
            return;
        }
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }
        player.weaponVisuals.PlayerFireAnimation();
        //�礶�������׹����������ԧ��ҧ����顴���Ф��駶֧�ԧ����ع
        if (currentWeapon.shootType == ShootType.Single)
        {
            isShooting = false; //���isShooting��false ����ѹ�������true�����ҨФ�ԡ���ҫ����ա����    
        }
        if (currentWeapon.BurstActivated() == true) //��һ׹����Դ��ҹburst��������ԧ�ѵ��ѵԵ����
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
        TriggerEnemyDodge();




    }//����ԧ�׹

    private void FireSingleBullet()
    {
        currentWeapon.ammoInMagazine--;
        // ��ҡ���ع�ҨҡBulletPool�����Ҥ�����
        GameObject newBullet = Object_Pool.instance.GetObject(bulletPrefab,GunPoint());
        
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);


        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance,currentWeapon.bulletDamage ,bulletImpactForce);

        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        //��˹�mass������ع����������ع����������˹����㹢�з���ѧ���Ѻ�ѵ�ش�������������Ѻ��������

        rbNewBullet.velocity = bulletDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    #region Slots Manager(Equip,Pickup,Drop,ReadyWeapon)
    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count) //��Ҿ�����������¹�׹���������������ù
        {
            return;
        }
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];

        player.weaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }

    public void PickupWeapon(Weapon newWeapon)
    {
       
        if (WeaponInInventory(newWeapon.weaponType) != null)
        {
            WeaponInInventory(newWeapon.weaponType).totalReserveAmmo += newWeapon.ammoInMagazine;
            return;
        }
        //������͵�����л׹�����������׹��������ӡ������¹�׹(���繶��)
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            DropWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }
        //�����������ظ���㹡�����������������������������ظ����ŧ������
        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();


    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) //��������ظ1��鹷�����ظ�����
        {
            return;
        }
        DropWeaponOnTheGround();
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0); //���������ظ��������ա��ͧ������������
    }

    private void DropWeaponOnTheGround()
    {
        GameObject dropWeapon = Object_Pool.instance.GetObject(weaponPickupPrefab,transform);
        dropWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    //SetWeaponReady�����������ظ��Ҿ�����ѧ��Ҿ��������������鹺��׹,����ö����Ŵ����ع��
    //����������ö�ԧ��������Ŵ����ع��͹����ѧ��Ժ�׹������������
    public void SetWeaponReady(bool ready) => weaponIsReady = ready; //��ʶҹ�weaponIsready
    public bool WeaponReady() => weaponIsReady;
    #endregion
    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
    }

    public Vector3 BulletDirection() //��ȷҧ�ͧ����ع    
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0;

        }

        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim); �ҷ���ҧ����

        return direction;
    }

    //�觵��˹�gunpoint�ͧ�׹�Ѩ�غѹ������
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1; //����ҵ͹�������ͻ׹�ѹ����㹡������ֻ���
    public Weapon WeaponInInventory(WeaponType weaponType) //����������ظ���㹡���������
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }
    private void TriggerEnemyDodge()
    { //����ѵ���ź������ն�ҵ͹����ԧ������ѵ�������ҧ˹��
        Vector3 rayOrigin = GunPoint().position; 
        Vector3 rayDirection = BulletDirection();
        if(Physics.Raycast(rayOrigin,rayDirection,out RaycastHit hit,Mathf.Infinity))
        {
            Enemy_Melee enemy_Melee = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();  
            if (enemy_Melee != null)
            {
                enemy_Melee.ActiveDodgeRoll();
            }
        }

    }


    #region InputEvent
    private void AssignInputEvent()
    {
        PlayerControll controls = player.controls;
        player.controls.Character.Fire.performed += context => isShooting = true;
        player.controls.Character.Fire.canceled += context => isShooting = false;
        player.controls.Character.ToggleMode.performed += context => currentWeapon.ToggleBurst();


        controls.Character.EquipSlot1.performed += context => EquipWeapon(0); //������1��������͵���ظ�á
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);


        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

    }

    #endregion

}

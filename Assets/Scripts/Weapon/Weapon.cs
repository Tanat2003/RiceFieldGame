//ʤ�Ի�������红����Ż׹
using UnityEngine;

public enum WeaponType //enum������纻������ͧ���ظ
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}
public enum ShootType
{
    Single,
    Auto
}

[System.Serializable] //�����ء���ҧ㹤���Weapon�١�ͧ����ʤ�Ի����蹷�����¡�� ��˹��inspector
public class Weapon
{
    #region Weapon Variables
    [Header("Weapon Detail")]
    public WeaponType weaponType;
    public int ammoInMagazine; //����ع�Ѩ�غѹ�������� 2/30
    public int magazineCapacity; //��Ҵ�ͧ��硡Ыչ
    public int totalReserveAmmo; //�ӹǹ����ع�������ͧ�׹����������
    public int bulletDamage;
    #endregion

    #region ShootVariables
    [Header("Shoot Detail")]
    public ShootType shootType;
    public int bulletPerShot { get; private set; } //����ع������͡仵��1����
    public float defaultFireRate;
    public float fireRate = 1;//����ع����Թҷ�
    private float lastShootTime;
    #endregion
    #region ReloadSpeed/GunDistance/CameraDistance/EquipSpeed Variables
    public float reloadSpeed { get; private set; } //��������㹡������Ŵ����ع  
    public float equipSpeed { get; private set; } //��������㹡����Ժ���ظ   
    public float gunDistance { get; private set; } //���С���ع�������ö����������л׹   
    public float cameraDistance { get; private set; }
    #endregion


    #region Spread Variables
    private float baseSpread = 1;
    private float currentSpread = 2;
    private float maximumSpread = 3;
    private float spreadIncreaseRate = .25f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    #endregion
    #region Burst Variables
    [Header("BrustFire")]
    private bool burst_Avalible;
    public bool burst_Active;

    public float burst_FireDelay { get; private set; } //Delay㹡���ԧ����ع����Burst���йѴ
    public float burst_ModeFireRate;//����ع����Թҷ�
    private int burst_ModeBulletPerShot;
    #endregion
    public Weapon(WeaponData weaponData) //constructor�ͧ�����Ż׹
    {
        weaponType= weaponData.weaponType;

        fireRate = weaponData.fireRate;
        defaultFireRate = fireRate;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        burst_Avalible = weaponData.burstAvalible;
        burst_Active = weaponData.burstActive;
        burst_ModeBulletPerShot = weaponData.burstBulletPerShot;
        burst_ModeFireRate = weaponData.burstFireRate;
        burst_FireDelay = weaponData.burstFireDelay;

        bulletPerShot= weaponData.bulletPerShot;
        shootType = weaponData.shootType;
        bulletDamage = weaponData.bulletDamage;

        ammoInMagazine = weaponData.ammoInMagazine;
        totalReserveAmmo = weaponData.totalReserveAmmo;
        magazineCapacity = weaponData.magazineCapacity;
        this.weaponData = weaponData;

    }



    #region Spread Method
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();
        float randomValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomValue, randomValue, randomValue);

        //���˹觡����ع��Ш��Ẻ���� * ���˹觡���ع�ҡgunpoint
        return spreadRotation * originalDirection;
    }

    private void IncreaseSpread()
    {
        //mathf.clamp�������Թ��Ҩ������ѹ���Թ����٧�ش(��ҷ�����,��ҵ���ش,����٧�ش)
        currentSpread =
            Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    private void UpdateSpread()
    {

        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }
        lastSpreadUpdateTime = Time.time;
    }
    #endregion
    #region Brust Method
    public bool BurstActivated()
    {
        if(weaponType == WeaponType.Shotgun) //����繻׹�١�ͧ����ԧẺburst���
        {
            burst_FireDelay = 0;
            return true;
        }
        return burst_Active;
    }
    public void ToggleBurst()
    {
        //������ظ�����ҹburstmode������������Թ
        //���������Ҩе�駤�ҡ���ع���shot���firerate������������burstmode
        //������ظ�����burstmode����������Դ����駤��bulletpershot���firerate�繤���������
        if (burst_Avalible == false)
        {
            return;
        }
        burst_Active = !burst_Active;
        if (burst_Active)
        {
            bulletPerShot = burst_ModeBulletPerShot;
            fireRate = burst_ModeFireRate;
        }else
        {
            bulletPerShot = 1;
            fireRate = defaultFireRate;
        }
    }



    #endregion
    public WeaponData weaponData { get; private set; }
    public bool CanShoot() => HaveEnoughBullet() && ReadyTofire();
  

    private bool ReadyTofire()
    {
        // lastShoottime+1 / fireRate = ����ع�������ԧ����1�Թҷ�
        //�� 1/4 = 0.25 ���¤��������Ҩ��ԧ����ع��ء�0.25�� �1���ԧ��4�Ѵ
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time; //���ҷ���ԧ�����ش��ҡѺ���һѨ�غѹ���method���ӧҹ
            return true;
        }
        return false;
    }
    #region ReloadBullet

    private bool HaveEnoughBullet() => ammoInMagazine > 0;//����ع�����ҡ����0return true


    public bool CanReload()
    {
        if (ammoInMagazine == magazineCapacity)
        {
            return false;
        }
        if (totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }
    public void RefillBullets()
    {
        totalReserveAmmo += ammoInMagazine;  //�����������Ŵ���ǡ���ع�Ѩ�غѹ����������������㹡���ع��������������
        int bullettoReload = magazineCapacity;

        if (bullettoReload > totalReserveAmmo) //��Ҩӹǹ����ع��������Ŵ���ҡ���ҡ���ع��������
        {
            bullettoReload = totalReserveAmmo;    //������ع��������Ŵ�բ�Ҵ��ҡѺ����ع����������������������Ŵ������������
        }
        totalReserveAmmo -= bullettoReload; //�ӹǹ����ع�٧�ش��������ź�͡���¨ӹǹ����ع�������Ŵ
        ammoInMagazine = bullettoReload; //����ع�Ѩ�غѹ=����ع�������Ŵ�
        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
    #endregion
}



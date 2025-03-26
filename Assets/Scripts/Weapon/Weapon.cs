//สคริปต์นี้ใช้เก็บข้อมูลปืน
using UnityEngine;

public enum WeaponType //enumที่ใช้เก็บประเภทของอาวุธ
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

[System.Serializable] //ทำให้ทุกอย่างในคลาสWeaponถูกมองเห็นในสคริปต์อื่นที่เรียกใช้ บนหน้าinspector
public class Weapon
{
    #region Weapon Variables
    [Header("Weapon Detail")]
    public WeaponType weaponType;
    public int ammoInMagazine; //กระสุนปัจจุบันที่เรามี 2/30
    public int magazineCapacity; //ขนาดของแม็กกะซีน
    public int totalReserveAmmo; //จำนวนกระสุนทั้งหมดของปืนนี้ที่เรามี
    public int bulletDamage;
    #endregion

    #region ShootVariables
    [Header("Shoot Detail")]
    public ShootType shootType;
    public int bulletPerShot { get; private set; } //กระสุนที่พุ่งออกไปต่อ1ครั้ง
    public float defaultFireRate;
    public float fireRate = 1;//กระสุนต่อวินาที
    private float lastShootTime;
    #endregion
    #region ReloadSpeed/GunDistance/CameraDistance/EquipSpeed Variables
    public float reloadSpeed { get; private set; } //ความเร็วในการรีโหลดกระสุน  
    public float equipSpeed { get; private set; } //ความเร็วในการหยิบอาวุธ   
    public float gunDistance { get; private set; } //ระยะกระสุนที่สามารถพุ่งไปได้ในแต่ละปืน   
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

    public float burst_FireDelay { get; private set; } //DelayในการยิงกระสุนโหมดBurstแต่ละนัด
    public float burst_ModeFireRate;//กระสุนต่อวินาที
    private int burst_ModeBulletPerShot;
    #endregion
    public Weapon(WeaponData weaponData) //constructorของข้อมูลปืน
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

        //ตำแหน่งการหมุนกระจายแบบสุ่ม * ตำแหน่งกระสุนจากgunpoint
        return spreadRotation * originalDirection;
    }

    private void IncreaseSpread()
    {
        //mathf.clampคือรีเทรินค่าจนกว่ามันจะเกินค่าสูงสุด(ค่าที่จะส่ง,ค่าต่ำสุด,ค่าสูงสุด)
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
        if(weaponType == WeaponType.Shotgun) //ถ้าเป็นปืนลูกซองให้ยิงแบบburstเลย
        {
            burst_FireDelay = 0;
            return true;
        }
        return burst_Active;
    }
    public void ToggleBurst()
    {
        //ถ้าอาวุธนี้ใช้งานburstmodeไม่ได้ให้รีเทริน
        //ถ้าใช้ได้เราจะตั้งค่ากระสุนต่อshotและfirerateใหม่ให้อยู่ในburstmode
        //ถ้าอาวุธนี้ใช้burstmodeได้แต่ไม่ได้เปิดให้ตั้งค่าbulletpershotและfirerateเป็นค่าเริ่มต้น
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
        // lastShoottime+1 / fireRate = กระสุนที่เรายิงได้ต่อ1วินาที
        //เช่น 1/4 = 0.25 หมายความว่าเราจะยิงกระสุนได้ทุกๆ0.25วิ ใน1วิยิงได้4นัด
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time; //เวลาที่ยิงไปล่าสุดเท่ากับเวลาปัจจุบันที่methodนี้ทำงาน
            return true;
        }
        return false;
    }
    #region ReloadBullet

    private bool HaveEnoughBullet() => ammoInMagazine > 0;//กระสุนในแม็กมากกว่า0return true


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
        totalReserveAmmo += ammoInMagazine;  //ให้เวลารีโหลดแล้วกระสุนปัจจุบันไม่หายไปเพราะไปเพิ่มในกระสุนทั้งหมดที่เรามี
        int bullettoReload = magazineCapacity;

        if (bullettoReload > totalReserveAmmo) //ถ้าจำนวนกระสุนที่จะรีโหลดมีมากกว่ากระสุนที่เรามี
        {
            bullettoReload = totalReserveAmmo;    //ให้กระสุนที่จะรีโหลดมีขนาดเท่ากับกระสุนที่เรามีเพื่อให้เรารีโหลดทีเดียวหมดเลย
        }
        totalReserveAmmo -= bullettoReload; //จำนวนกระสุนสูงสุดที่เรามีลบออกด้วยจำนวนกระสุนที่รีโหลด
        ammoInMagazine = bullettoReload; //กระสุนปัจจุบัน=กระสุนที่รีโหลดไป
        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
    #endregion
}



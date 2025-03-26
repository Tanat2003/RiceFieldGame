using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player_WeaponVisual : MonoBehaviour
{



    private Animator anim;
    private Player player;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModels[] backupModels;



    [Header("Rig")]
    [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased = true;
    private Rig rig;

    [Space]
    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private float leftHandIK_IncreaseSpeed;
    private bool leftHandIK_ShouldBeIncreased;


    [Header("LaserPointPosition")]
    [SerializeField] private Transform laserPointTarget;





    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        backupModels = GetComponentsInChildren<BackupWeaponModels>(true);
        weaponModels = GetComponentsInChildren<WeaponModel>(true);//เก็บทุกChildrenที่มีWeaponModelในComponentต่อให้Enableเป็นfalse

    }

    private void Update()
    {


        UpdateRigWeight();
        UpdateLeftHandIKWeight();

    }
    public void PlayerFireAnimation() => anim.SetTrigger("Fire");
    public WeaponModel CurrentWeaponModel() //เอาไว้ใช้รีเทรินค่าประเภทอาวุธเพื่อเรียกใช้ในสคริปต์อื่นๆภายหลัง
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            //ถ้าประเภทอาวุธในปัจจุบันมีค่าเท่ากับอาวุธไหนในสคริปต์ ให้weaponmodelในฟังชั่นนี้เท่ากับอาวุธในลำดับที่เท่านั้น
            //ที่มันเท่ากับอาวุธที่ถือในปัจจุบัน
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }
        return weaponModel;
    }



    public void PlayReloadAnimation()
    {

        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");

        ReduceLeftHandIKWeight();
    }
    #region Rig_Animation
    private void UpdateLeftHandIKWeight()
    {
        if (leftHandIK_ShouldBeIncreased)
        {
            leftHandIK.weight += leftHandIK_IncreaseSpeed * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                leftHandIK_ShouldBeIncreased = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;
            if (rig.weight >= 1)
            {
                rigShouldBeIncreased = false;
            }


        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }
    private void ReduceLeftHandIKWeight()
    {
        leftHandIK.weight = 0.15f;
    }

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true; //เซ็ทค่าตัวแปรให้เรารีเทรินค่าrigweightเป็น1เพื่อให้มือซ้ายอยู่ในท่าที่ถูกต้อง
    private void AttachLeftHand() //รับตำแหน่งLefthandที่เราตั้งpositionไว้ให้มาเล่นตอนจับอาวุธ
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;


        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;




    }
    public void ReturnLeftHandIKWeight() => leftHandIK_ShouldBeIncreased = true;
    #endregion



    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffBackupWeaponModels();
        SwitchOffWeaponModels();

        if (player.weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackupWeaponModel();

        }


        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);


        AttachLeftHand();
    }//สลับปืน

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }

    }//ไม่โชว์ปืนที่ไม่ได้ใช้

    private void SwitchOffBackupWeaponModels() //ปิดไม่ให้เห็นbackupโมเดลอาวุธที่ผู้เล่นไม่มี
    {
        foreach (BackupWeaponModels backupModel in backupModels)
        {
            backupModel.Activate(false);
        }

    }
    public void SwitchOnBackupWeaponModel()//เปิดให้เห็นbackupโมเดลอาวุธที่ผู้เล่นไม่มี
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModels lowHangWeapon = null;
        BackupWeaponModels backHangWeapon = null;
        BackupWeaponModels sideHangWeapon = null;

        foreach (BackupWeaponModels backupModel in backupModels)
        {
            //เช็คว่าอาวุธในbackupโมเดลอยู่ในกระเป๋าผู้เล่นมั้ยถ้าอยู่อาวุธนั้นมีตำแหน่งการวางไว้ตรงไหนของผู้เล่น
            
            if(backupModel.weapontype == player.weapon.CurrentWeapon().weaponType)
            {
                continue;
            }
            if (player.weapon.WeaponInInventory(backupModel.weapontype) != null)
            {
                if (backupModel.HangtypeIs(HangType.LowBackHang))
                {
                    lowHangWeapon = backupModel;
                }
                if(backupModel.HangtypeIs(HangType.BackHang))
                {
                    backHangWeapon = backupModel;
                }
                if(backupModel.HangtypeIs(HangType.SideHang))
                {
                    sideHangWeapon = backupModel;
                }


            }

        }
        //ตัวแหน่งของbackupโมเดลอันไหนไม่เป็นnullให้เปิดactivateให้ผู้เล่นเห็น
        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);

    }


    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        anim.SetLayerWeight(layerIndex, 1);
    }//เปลี่ยนlayerของอนิเมชั่น

    public void PlayWeaponEquipAnimation() //เล่นอนิเมชั่นหยิบปืน
    {
        EquipType EquipType = CurrentWeaponModel().equipAnimationType;
        float equipmentSpeed = player.weapon.CurrentWeapon().equipSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)EquipType)); //เซ็ทค่าblentreeตามgrabtypeที่ส่งมาให้
        anim.SetFloat("EquipSpeed", equipmentSpeed);




    }







}


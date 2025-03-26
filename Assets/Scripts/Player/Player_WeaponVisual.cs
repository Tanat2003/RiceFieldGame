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
        weaponModels = GetComponentsInChildren<WeaponModel>(true);//�纷ءChildren�����WeaponModel�Component������Enable��false

    }

    private void Update()
    {


        UpdateRigWeight();
        UpdateLeftHandIKWeight();

    }
    public void PlayerFireAnimation() => anim.SetTrigger("Fire");
    public WeaponModel CurrentWeaponModel() //������������Թ��һ��������ظ�������¡���ʤ�Ի�����������ѧ
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            //��һ��������ظ㹻Ѩ�غѹ�դ����ҡѺ���ظ�˹�ʤ�Ի�� ���weaponmodel㹿ѧ��蹹����ҡѺ���ظ��ӴѺ�����ҹ��
            //����ѹ��ҡѺ���ظ�����㹻Ѩ�غѹ
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

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true; //�緤�ҵ��������������Թ���rigweight��1���������ͫ�������㹷�ҷ��١��ͧ
    private void AttachLeftHand() //�Ѻ���˹�Lefthand�����ҵ��position����������蹵͹�Ѻ���ظ
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
    }//��Ѻ�׹

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }

    }//������׹����������

    private void SwitchOffBackupWeaponModels() //�Դ���������backup�������ظ�������������
    {
        foreach (BackupWeaponModels backupModel in backupModels)
        {
            backupModel.Activate(false);
        }

    }
    public void SwitchOnBackupWeaponModel()//�Դ������backup�������ظ�������������
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModels lowHangWeapon = null;
        BackupWeaponModels backHangWeapon = null;
        BackupWeaponModels sideHangWeapon = null;

        foreach (BackupWeaponModels backupModel in backupModels)
        {
            //��������ظ�backup��������㹡����Ҽ��������¶���������ظ����յ��˹觡���ҧ���ç�˹�ͧ������
            
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
        //����˹觢ͧbackup�����ѹ�˹�����null����Դactivate�����������
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
    }//����¹layer�ͧ͹������

    public void PlayWeaponEquipAnimation() //���͹��������Ժ�׹
    {
        EquipType EquipType = CurrentWeaponModel().equipAnimationType;
        float equipmentSpeed = player.weapon.CurrentWeapon().equipSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)EquipType)); //�緤��blentree���grabtype����������
        anim.SetFloat("EquipSpeed", equipmentSpeed);




    }







}


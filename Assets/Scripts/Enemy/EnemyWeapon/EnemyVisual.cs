using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMeleeWeaponType {Hold, Throw,NoWeapon }

public class EnemyVisual : MonoBehaviour
{   //�����ѡɳТͧ�ѵ��

    [Header("Weapon Visual")]
    [SerializeField] private EnemyWeaponModel[] weaponModels;
    [SerializeField] private EnemyMeleeWeaponType weaponType;
    public GameObject currentWeaponModel { get; private set; }

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("CrysTal Visual")]
    [SerializeField] private GameObject[] crystal;
    [SerializeField] private int crystalAmount;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
        FindCrystalPosition();
       // InvokeRepeating(nameof(SetUpEnemyLook), 0, 1.5f);
        
    }
    public void EnableWeaponTrail(bool eneable)
    {
        EnemyWeaponModel currentWeaponScript = currentWeaponModel.GetComponent<EnemyWeaponModel>();
        currentWeaponScript.EnableTrailEffects(eneable);
    }

    private void SetupRandomCrystal()
    {
        
        List<int> avalibleIndex = new List<int>();
        for (int i = 0; i < crystal.Length; i++) //������ҵ���ӹǹ���ʵ��ŷ���������list
        {
            avalibleIndex.Add(i);
            crystal[i].SetActive(false);
        }

        for (int i = 0;i < crystalAmount; i++)
        {
            if(avalibleIndex.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0,avalibleIndex.Count);
            int objectIndex = avalibleIndex[randomIndex]; //�list�������ö�Դcrystyal���� �Ҩҡ���˹�randomIndex

            crystal[objectIndex].SetActive(true); //�Դ��������crystal���
            avalibleIndex.RemoveAt(randomIndex); //źcrystal���Դ�������͡�ҡ��ʵ�
        }

    }

    private void FindCrystalPosition()
    {
        Enemy_Crystal[] crystalComponent = GetComponentsInChildren<Enemy_Crystal>(true);
        crystal = new GameObject[crystalComponent.Length];
        for (int i = 0; i < crystalComponent.Length; i++)
        {
            crystal[i] = crystalComponent[i].gameObject;
        }
        crystalAmount = Random.Range(0,crystalComponent.Length);
    }

    public void SetUpEnemyLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCrystal();
    }
    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);
        Material randomMaterial = new Material(skinnedMeshRenderer.material);
        randomMaterial.mainTexture = colorTextures[randomIndex];
        skinnedMeshRenderer.material = randomMaterial;

    }
    private void SetupRandomWeapon()
    {
        foreach (var weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }
        List<EnemyWeaponModel> filterWeaponModels = new List<EnemyWeaponModel>();
        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                filterWeaponModels.Add(weaponModel);
            }
        }
        int randomIndex = Random.Range(0, filterWeaponModels.Count);
       // filterWeaponModels[randomIndex].gameObject.SetActive(true);
        currentWeaponModel = filterWeaponModels[randomIndex].gameObject;
        currentWeaponModel.SetActive(true);
        OverrideAnimatorController();
    }

    private void OverrideAnimatorController()
    {
        //��������ظ������� AnimatorOverrideController �����OverrideAnimator���᷹

        AnimatorOverrideController overrideController =
                    currentWeaponModel.GetComponent<EnemyWeaponModel>().overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    public void SetupWeaponType(EnemyMeleeWeaponType type) => weaponType = type;
    

    
    
}

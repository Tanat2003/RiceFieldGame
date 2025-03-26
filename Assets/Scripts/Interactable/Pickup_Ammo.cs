using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AmmoData //struct เป็นDatastrucที่เก็บตัวแปรที่ตัวแปรเก็บค่าข้างในอีกทีได้(เก็บตัวแปรที่ค่าข้างในเปลี่ยนแปลงได้)
{
    public WeaponType WeaponType;//ประเภทปืน
    [Range(10, 100)]
    public int minAmount;
    [Range(10, 100)]
    public int maxAmount;
}

public enum AmmoBoxType { smallBox,bigBox}
public class Pickup_Ammo : Interactable
{
    
    [SerializeField] private AmmoBoxType boxType;
    
    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] public List<AmmoData> bigBoxAmmo;
    [SerializeField] private GameObject[] boxModel;
    private void Start()
    {
        SetupBoxModel();

    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            if (i == ((int)boxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }

    public override void InterAction()
    {
       List<AmmoData> currentAmmoList = smallBoxAmmo;
        if(boxType == AmmoBoxType.bigBox)
        {
            currentAmmoList = bigBoxAmmo;

                
        }
        foreach(AmmoData ammo in currentAmmoList)
        {
            Weapon weapon =weaponController.WeaponInInventory(ammo.WeaponType);
            AddBulletsToWeapon(weapon,GetBulletAmount(ammo));
        }
        Object_Pool.instance.ReturnObject(gameObject);

    }

    private void AddBulletsToWeapon(Weapon weapon, int amountBullet)
    {
        if (weapon == null)
        { return; }
        weapon.totalReserveAmmo += amountBullet;
    }
   
    private int GetBulletAmount(AmmoData ammo)
    {
        //mathf.minสุ่มค่าที่น้อยที่สุดจากสองค่า
        //mathf.maxสุ่มค่ามากที่สุดจากสองค่า
        //กันเหนียวเผื่อใส่สลับเลขmaxกับminในinspector
        float min = Mathf.Min(ammo.minAmount,ammo.maxAmount);
        float max = Mathf.Max(ammo.minAmount,ammo.maxAmount);

        float randomAmmoAmount = Random.Range(min,max);
        return Mathf.RoundToInt(randomAmmoAmount); //เเปลงค่าทศนิยมให้เป็นตัวเลข  
    }

}

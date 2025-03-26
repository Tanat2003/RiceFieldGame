using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Interactable
{
    
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private BackupWeaponModels[] models;
    [SerializeField] private Weapon weapon;

    private bool oldWeapon;

    private void Start()
    {
        if(oldWeapon == false)
        {
            weapon = new Weapon(weaponData);
        }
        SetupGameObject();
    } 
    public void SetupPickupWeapon(Weapon weapon,Transform transform)
    {
        oldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.weaponData;
        this.transform.position = transform.position+new Vector3(0,0.75f,0);
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        SetupWeaponPickupModel();
    }
    private void SetupWeaponPickupModel()
    {
        foreach(BackupWeaponModels model in models)
        {
            model.gameObject.SetActive(false);
            if(model.weapontype == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }
    public override void InterAction()
    {
        weaponController.PickupWeapon(weapon);
        Object_Pool.instance.ReturnObject(gameObject);
        
    }

 

}

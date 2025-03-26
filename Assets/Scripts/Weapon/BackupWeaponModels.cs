using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HangType { LowBackHang,BackHang,SideHang} //µÓáË¹è§·Õè¨ÐËÂÔº»×¹ÍÍ¡ÁÒ

public class BackupWeaponModels : MonoBehaviour
{
    public WeaponType weapontype;
    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);
    //¶éÒà§×èÍä¢¹Õéà»ç¹¨ÃÔ§return true
    public bool HangtypeIs(HangType hangType)=> this.hangType == hangType;
}

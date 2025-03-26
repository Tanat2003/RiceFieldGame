using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HangType { LowBackHang,BackHang,SideHang} //���˹觷�����Ժ�׹�͡��

public class BackupWeaponModels : MonoBehaviour
{
    public WeaponType weapontype;
    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);
    //�������䢹���繨�ԧreturn true
    public bool HangtypeIs(HangType hangType)=> this.hangType == hangType;
}

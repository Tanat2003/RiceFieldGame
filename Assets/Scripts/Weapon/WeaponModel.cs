using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipType { SideEquipAnimation, BackEquipAnimation }; //ประกาศค่าคงที่เป็นenum SideGrab=0 BackGrab=1
public enum HoldType { CommonHold=1,LowHold,HighHold}//เปลี่ยนindexของenumเพราะเราจะให้กำหนดLayerIndexของอนิเมชั่น
public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint; //ตำแหน่งกระสุน
    public Transform holdPoint; //lefthandIK



}

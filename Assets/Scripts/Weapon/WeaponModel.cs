using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipType { SideEquipAnimation, BackEquipAnimation }; //��С�Ȥ�Ҥ������enum SideGrab=0 BackGrab=1
public enum HoldType { CommonHold=1,LowHold,HighHold}//����¹index�ͧenum������Ҩ�����˹�LayerIndex�ͧ͹������
public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint; //���˹觡���ع
    public Transform holdPoint; //lefthandIK



}

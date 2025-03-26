using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControll controls {  get; private set; }
    
    public Player_Aim aim { get; private set; } //get private set คืออ่านได้อย่างเดียวเท่านั้น
    public Player_Movement movement { get; private set; }
    public Player_WeaponController weapon { get; private set; }
    public Player_WeaponVisual weaponVisuals {  get; private set; }
    public Player_Interaction interaction { get; private set; }
    public Player_Health health { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public Animator animator { get; private set; }
    
    private void Awake() //เข้าถึงสคริปต์ต่างๆ
    {
        controls = new PlayerControll();

        animator = GetComponentInChildren<Animator>();    
        ragdoll = GetComponent<Ragdoll>();
        health = GetComponent<Player_Health>();
        aim= GetComponent<Player_Aim>();
        movement = GetComponent<Player_Movement>();
        weapon = GetComponent<Player_WeaponController>();
        weaponVisuals = GetComponent<Player_WeaponVisual>();
        interaction = GetComponent<Player_Interaction>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }


}

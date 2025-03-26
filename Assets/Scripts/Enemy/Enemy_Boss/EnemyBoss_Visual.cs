using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBoss_Visual : MonoBehaviour
{

    private EnemyBoss enemy;
    [SerializeField] private float landingoffset = 1;
    [SerializeField] private ParticleSystem landingZone;
    [SerializeField] private GameObject[] weaponTrail;
    [Header("Battery")]
    [SerializeField] private GameObject[] batteries; //แบตเตอรี่ชาร์จให้ผู้เล่นเห็นว่าเมื่อไหร่บอสพร้อมจะปล่อยพ่นไฟ
    [SerializeField] private float initalBatteryScaleY = 0.15468f; //ขนาดของObjBattery


    private float dischargeSpeed;//ความเร็วในการลดเเบตเตอรี่(ลดตอนใช้สกิล)
    private float rechargeSpeed; //ความเร็วในการชาร์จแบตเตอรี่
    private bool isRecharging;

    private void Awake()
    {
        enemy = GetComponent<EnemyBoss>();
        landingZone.transform.parent =null;
        landingZone.Stop();
        ResetBatteries();
    }
    private void Update()
    {
        
       UpdateBatteryScale();
    }
    public void PlaceLandingZone(Vector3 target)
    {
        Transform plane = GameObject.Find("Plane").GetComponent<Transform>();
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * landingoffset;
        

        landingZone.transform.position = target+ offset + plane.position + new Vector3(0,0.1f,0) ;
        
        landingZone.Clear();

        var mainModule = landingZone.main;
        mainModule.startLifetime = enemy.travelTimeToTarget*2;
        landingZone.Play();
    }
    private void UpdateBatteryScale()
    {
        if(batteries.Length <=0)
        {
            return;
        }
        foreach(GameObject batteries in batteries)
        {
            if(batteries.activeSelf)
            {
                float scaleChange = (isRecharging? rechargeSpeed: -dischargeSpeed)*Time.deltaTime;
                float newScaleY =
                    Mathf.Clamp(batteries.transform.localScale.y + scaleChange, 0, initalBatteryScaleY);
                batteries.transform.localScale = new Vector3(0.15f, newScaleY, .15f);
                if(batteries.transform.localScale.y <=0)
                {
                    batteries.SetActive(false);
                }

            }
        }
    }
    public void ResetBatteries()
    {
        isRecharging = true;
        rechargeSpeed = initalBatteryScaleY/enemy.abilityCooldown;
        dischargeSpeed = initalBatteryScaleY/(enemy.flameThrowDuration*.75f);
        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }

    public void DischargeBattery() => isRecharging = false;
    public void EnableWeaponTrail(bool active)
    {
        if(weaponTrail.Length <=0)
        {
            return;
        }
        foreach (GameObject trail in weaponTrail)
        { 
            trail.SetActive(active);
        }
    }
}

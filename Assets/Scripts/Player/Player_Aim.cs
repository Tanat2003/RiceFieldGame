using System;
using UnityEngine;

public class Player_Aim : MonoBehaviour
{


    private Player player;
    private PlayerControll controls;
    private Vector2 mouseInput;

    [Header("Aim Visual-Lase")]
    [SerializeField] private LineRenderer aimLaser;


    [Header("Aim controll")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingToTarget;


    [Header("Cameara control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0f, 1f)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 4.3f;
    [SerializeField] private float cameraSensetivty = 5f;

    [Space]
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;
    private RaycastHit lastKnowMouseHit;


    private void Start()
    {
        player = GetComponent<Player>();
        

        AssignInputEvents();
    }
    private void Update()
    {
        if (player.health.isDead)
        {
            return;
            
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisly = !isAimingPrecisly;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;
        }


        UpdateAimPosition();
        UpdateCameraPosition();
        UpdateAimVisual();
    }

    private void UpdateAimPosition() //อัปเดตตำแหน่งเป้าเล็ง
    {
        Transform target = Target();

        if (target != null && isLockingToTarget) //ถ้าเป้าเล็งที่targetและเปิดโหมดช่วยเล็งไว้ให้ ล็อกเป้าไปที่เป้าหมาย
        {

            if (target.GetComponent<Renderer>() != null) //เช็คว่ามีRenderมั้ยถ้ามีให้aimposition = ตรงกลางของrender
            {
                aim.position = target.GetComponent<Renderer>().bounds.center;


            }
            else
            {
                aim.position = target.position;

            }
            return;
        }



        aim.position = GetMouseHitInfo().point;
        if (!isAimingPrecisly)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }
    private void UpdateAimVisual()
    {

        aimLaser.enabled = player.weapon.WeaponReady();
        if(aimLaser.enabled == false) //ถ้าaimlaserenableเป็นfalseแปลว่ายังจับปืนหรือยังรีโหลดกระสุนไม่เสร็จ
        {
            return;
        }
        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserLenght = .5f;
        float gunDistance = player.weapon.CurrentWeapon().gunDistance;
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        //Physics.Raycast(จุดเริ่มต้น,จุดปลาย,output ,ระยะทาง);  
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserLenght = 0;
        }

        //เซ็ทตำแหน่งtrailrenderer
        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserLenght);
    }//อัปเดตตำแหน่งเลเซอร์เป้าเล็ง


    public Transform Aim() => aim; // return ตำแหน่งaimไป
    public Transform Target()//ล็อกเป้าให้ผู้เล่น
    {
        Transform target = null;
        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;

        }
        return target;
    }


    public bool CanAimPrecisly() => isAimingPrecisly; //ฟังชั่นreturnค่าisAiming
    public RaycastHit GetMouseHitInfo() //เช็คตำแหน่งเม้าส์ว่าชนกับวัตถุอะไรมั้ยโดยใช้Raycast
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out var hitinfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnowMouseHit = hitinfo;
            
        }
        return lastKnowMouseHit;

    }

    private void AssignInputEvents()
    {
        controls = player.controls;
        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }


    #region CameraRegion 
    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivty * Time.deltaTime);
    }
    private Vector3 DesiredCameraPosition()
    {
        // ประกาศตัวแปร = xxx ? N:P หมายถึงซ้ายเท่ากับเงื่อนไขด้านขวาหรือไม่ถ้าใช่ เราใช้ค่าตัวแปร Nถ้าไม่ใช่ใช้ค่าตัวแปร P
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;



        Vector3 desiredCamearaPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCamearaPosition - transform.position).normalized;
        float distanceToDesierdPosition = Vector3.Distance(transform.position, desiredCamearaPosition);

        float clampDistance = Mathf.Clamp(distanceToDesierdPosition, minCameraDistance, actualMaxCameraDistance);
        // mathf.Clamp(ค่าที่ส่งให้ตัวแปร,ค่าน้อยสุดที่ไม่ให้ต่ำกว่านี้,ค่ามากสุดที่ไม่ให้เกิน)

        desiredCamearaPosition = transform.position + aimDirection * clampDistance;
        desiredCamearaPosition.y = transform.position.y + 1;



        return desiredCamearaPosition;
    }


    #endregion //รวมฟังชั่นที่เกี่ยวกับกล้อง



}

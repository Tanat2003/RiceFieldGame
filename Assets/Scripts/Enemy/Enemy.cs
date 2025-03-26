using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{

    public int healthPoints = 20;
    private Transform playerMain;
    public EnemyStateMachine stateMachine { get; private set; }
    public EnemyState idleState { get; private set; }
    public EnemyState moveState { get; private set; }
    public Animator animator { get; private set; }
    public Transform player { get; private set; }
    public Enemy_Health health { get; private set; }
    public EnemyVisual visual { get; private set; }
    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsAlly;
    
    [Space]

    [Header("StateInfo")]
    public float idleTime;
    public float agressionRange;

    [Header("MoveInfo")]
    public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private Vector3[] patrolPointPosition;
    [SerializeField] private Transform[] partrolPoints; //จุดหมายที่aiจะเดินไป
    private bool manualMovement;
    private bool manualRotation;

    private int currentPartrolIndex;
    public NavMeshAgent agent { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        visual = GetComponent<EnemyVisual>();
        health = GetComponent<Enemy_Health>();
        player = GameObject.Find("Player")?.transform.Find("player_main");

    }
    protected virtual void Start()
    {
        InitializePatrolPoints();
    }
    protected virtual void Update()
    {

    }
    #region PatrolPoint Method
    public Vector3 GetPartrolDestination()
    {
        Vector3 destination = patrolPointPosition[currentPartrolIndex];
        currentPartrolIndex++; //เพิ่มไว้รอใช้ชี้ตำแหน่งถัดไป
        if (currentPartrolIndex >= partrolPoints.Length)
        {
            currentPartrolIndex = 0;
        }
        return destination;
    }
    private void InitializePatrolPoints() //เอาตำแหน่งที่ให้aiเดินออกจากตัวaiเพื่อไม่ให้เวลาเดินแล้ว
                                         //ตำแหน่งเดินตามไปด้วย
    {

        patrolPointPosition = new Vector3[partrolPoints.Length];

        for (int i = 0; i < partrolPoints.Length; i++)
        {
            patrolPointPosition[i] = partrolPoints[i].position;
            partrolPoints[i].gameObject.SetActive(false);

        }

    }
    #endregion

    #region HitMethod
    public virtual void GetHit(int damage) //เช็คว่าenemyโดนยิง
    {
        health.ReduceHealth(damage);
        if(health.ShouldDie())
        {
            Die();
        }
        EnterBattleMode(); //ถ้าเราเรียกเมธอดในสคริตป์ที่มันอยู่มันจะไม่เรียกตัวเมธอดที่ถูกoverride
    }
    public virtual void Die()
    {

    }
    public  virtual void MeleeAttackCheck(Transform[] damagePoints,float attackCheckRadius,GameObject fx,int damage) //เช็คว่าอาวุธของEnemyโจมตีโดนผู้เล่นรึยังตอนเล่นอนิเมชั่นโจมตี
    {
        if (isMeleeAttackReady == false)
        {
            return;
        }
        foreach (Transform attackPoint in damagePoints)
        {
            // เก็บColliderเป็นอาเรย์ โดยใช้Physic Overlap(attackPointกับ Radiusของอาวุธ ว่าไปโดนเลเยอร์ผุ้เล่นมั้ย)
            Collider[] detectHits =
                Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);
            //ถ้าเราได้Colliderก็หยุดloop แล้วเรียกใช้TakeDamageในIdamagable
            for (int i = 0; i < detectHits.Length; i++)
            {
                IDamagable damagable = detectHits[i].GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(damage);
                    isMeleeAttackReady = false;
                    GameObject newAttackFx =
                        Object_Pool.instance.GetObject(fx, attackPoint);
                    Object_Pool.instance.ReturnObject(newAttackFx, 1);
                    return;
                }
            }


        }

    }
    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;

    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if(health.ShouldDie())
        {
            StartCoroutine(HitImpactCourutine(force, hitPoint, rb));

        }
        //ใช้Corutineเพื่อให้มีดีเลย์นิดหน่อยกันบัค
    }
    private IEnumerator HitImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);   //เพิ่มแรงกระแทกเข้าไปจุดที่เรายิงโดน 
    }
    #endregion

    #region Enter&ShouldEnterBattle
    protected bool ShouldEnterBattleMode()
    {
        bool inAgressionRange = Vector3.Distance(transform.position, player.position) < agressionRange;
        if (inAgressionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }
    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }
    #endregion

    #region Animation Method
    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();




    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualMovementActive() => manualMovement;
    public bool ManualRotationActive() => manualMovement;
    #endregion
    public void FaceTarget(Vector3 target,float turnSpeed =0) //หมุนให้หน้าenemyตรงกับtargetเป้าหมาย
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        float yRotation =
            Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);

    }
    protected virtual void OnDrawGizmos() //วาดขนาดระยะที่จะให้aiใช้detectผู้เล่น ถ้าเข้ามาในระยะให้Aiวิ่งไปหา
    {
        Gizmos.DrawWireSphere(transform.position, agressionRange);

    }

}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //�����struct ��������ҧ��ҧ����ö�ͧ����Inspector��
public struct MeleeAttackData
{
    public int attackDamage;
    public string attackName;
    public float attackRange;
    public float moveSpeed;//��������㹡������
    public float attackIndex; //index�ٻẺ�������
    [Range(1f, 2f)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}
public enum AttackType_Melee { Close, Charge } //�ٻẺ������բͧEnemyMelee
public enum EnemyMelee_Type { Regular, Shield, Dodge, Throw } //�������ͧenemyMelee

public class Enemy_Melee : Enemy
{
    public EnemyVisual visuals { get; private set; }

    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }

    public Ragdoll ragdoll { get; private set; }

    [Header("Enemy Setting")]
    public EnemyMelee_Type meleeType;

    [Header("Dodge")]
    public float dodgeCooldown;
    public float lastTimeDodge;

    [Header("Shield")]
    [SerializeField] private Transform shieldTransform; //���˹觧���
    public int shieldDurability;

    [Header("Item Throw Ability")]
    public GameObject throwPrefab;
    public float throwFlySpeed;
    public Transform throwStartPoint;
    public float throwTimer;
    public float abilityCooldown;
    public int throwDamage;
    private float lastTimeAbilityThrow;



    [Header("Attack Data")]

    public MeleeAttackData attackData;
    public List<MeleeAttackData> attackList;
    [SerializeField] private GameObject meleeAttackFX;
    private EnemyWeaponModel currentWeapon;
    private bool isAttackReady;




    protected override void Awake()
    {
        base.Awake();
        visuals = GetComponent<EnemyVisual>();
        ragdoll = GetComponent<Ragdoll>();


        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        abilityState = new AbilityState_Melee(this, stateMachine, "AttackThrow");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); //�����������������Pool��˹������Idlestate������

    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        InitializeSpciality();
        visuals.SetUpEnemyLook();
        UpdateAttackData();

    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();


        MeleeAttackCheck(currentWeapon.damagePoints,currentWeapon.attackRadius,meleeAttackFX,attackData.attackDamage);


        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }


    }

    #region Attack Method
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    public void UpdateAttackData()
    {
        currentWeapon = visuals.currentWeaponModel.GetComponent<EnemyWeaponModel>();

        if (currentWeapon != null)
        {
            attackList = new List<MeleeAttackData>(currentWeapon.weaponData.attackData);
        }
    }
    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return;
        }
        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    public override void Die()
    {
        base.Die();
        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }


    

    #endregion
    #region Ability Method&SetupMeleeType
    private void InitializeSpciality() //���Ѿ�ѵ�����л�������������ظ�������˹
    {
        if (meleeType == EnemyMelee_Type.Throw)
        {
            visuals.SetupWeaponType(EnemyMeleeWeaponType.Throw);
        }
        if (meleeType == EnemyMelee_Type.Shield)
        {
            animator.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            visuals.SetupWeaponType(EnemyMeleeWeaponType.Hold);
        }
        if (meleeType == EnemyMelee_Type.Dodge)
        {
            visuals.SetupWeaponType(EnemyMeleeWeaponType.NoWeapon);
        }


    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger(); //���¡Ability�ҡcurrentState ����AbilityState_Melee
        moveSpeed = moveSpeed * .5f;
        EnableWeaponModel(false);
    }
    public bool CanThrowAbility()
    {
        if (meleeType != EnemyMelee_Type.Throw)
        {
            return false;
        }
        if (Time.time > lastTimeAbilityThrow + abilityCooldown)
        {
            lastTimeAbilityThrow = Time.time;
            return true;
        }
        return false;
    }
    #endregion
    #region Active&EnableMethod
    public void ActiveDodgeRoll() //���������ö�ź+�����������Ҥ�Ŵ�ǹ�㹡�������ѧ
    {
        if (meleeType != EnemyMelee_Type.Dodge)
        {
            return;
        }

        if (stateMachine.currentState != chaseState)
        {
            return;
        }

        float dodgeAnimationDuration = GetAnimationClipDuration("DodgeRoll");
        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge) //Time.time =���ҵ�����������
        {// ����ö��Dodge��������� 1������Ҥ�Ŵ��dodge 2���͹������dodge�������
            lastTimeDodge = Time.time;
            animator.SetTrigger("DodgeRoll");

        }
    }
    private float GetAnimationClipDuration(string clipName) //Return���˹觪�ǧ�����AnimationClip
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.Log("Notfound" + clipName);
        return 0;
    }
    public void EnableWeaponModel(bool active) //����ѺEnemy��������ظ
    {
        visuals.currentWeaponModel.gameObject.SetActive(active);

    }
    #endregion
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

}

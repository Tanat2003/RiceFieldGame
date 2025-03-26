using System.Collections.Generic;
using UnityEngine;

public enum BossWeaponType { FireThrow, Hammer }
public class EnemyBoss : Enemy
{
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public SpeacialAttack1State_Boss speacialAttack1State { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public EnemyBoss_Visual bossVisual { get; private set; }
    public DeadState_Boss deadState { get; private set; }
    public Ragdoll ragdoll { get; private set; }

    [Header("Boss Detail")]
    public BossWeaponType bossWeaponType;
    public float actionCoolDown = 10;
    public float attackRange;


    [Header("Ability")]
    public float abilityCooldown;
    private float lastTimeUsedAbility;
    [SerializeField] private float minAbilityDistance;
    [Header("FlameThrow")]
    public ParticleSystem flameThrow;
    public float flameThrowDuration;
    public float flameDamageCooldown; //§Ÿ≈¥“«πÏ¥“‡¡®µÕπ∑’ËºŸÈ‡≈Ëπ‚¥π‰ø
    public int flameDamage;

    public bool flameThrowActive { get; private set; }
    [Header("Hammer")]
    public GameObject hammerFxPrefab;
    public int hammerActiveDamage;//¥“‡¡®¢Õß °‘≈
    [SerializeField] private float hammerCheckRadius; //¢π“¥¢Õß«ß °‘≈

    [Header("Jump attack/SpeacialAttack1")]
    public float travelTimeToTarget = 1;
    public float jumpAttackCooldown = 2;
    public int jumpAttackDamage;
    private float lastTimeJump;
    public float minJumpDistanceRequired;
    public Transform impactPoint;
    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 5;
    [SerializeField] private float upwardsMultiplier = 10;

    [Header("Attack Data")]
    public int meleeAttackDamage;
    [SerializeField] private Transform[] damagePoints;
    [SerializeField] private float attackRadius; //hitbox¢Õßattack∑’Ë‚®¡µ’ºŸÈ‡≈Ëπ
    [SerializeField] private GameObject meleeAttackFX;

    [Space]
    [SerializeField] private LayerMask whatToIgnore;

    protected override void Awake()
    {
        base.Awake();
        ragdoll = GetComponent<Ragdoll>();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        speacialAttack1State = new SpeacialAttack1State_Boss(this, stateMachine, "SpeacialAttack1");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle");

        bossVisual = GetComponent<EnemyBoss_Visual>();
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);


    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
        MeleeAttackCheck(damagePoints, attackRadius, meleeAttackFX,meleeAttackDamage);

    }
    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return;
        }
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public override void Die()
    {
        base.Die();
        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);

        }
    }
    public bool CanDoAbility()
    {
        bool playerWithInDistance =
            Vector3.Distance(transform.position, player.position) < minAbilityDistance;
        if (playerWithInDistance == flameThrow)
        {
            return false;
        }

        if (Time.time > lastTimeUsedAbility + abilityCooldown)
        {

            return true;
        }
        return false;
    }
    public void ActivateFlameThrow(bool activate)
    {
        flameThrowActive = activate;
        if (activate == false)
        {
            flameThrow.Stop();
            animator.SetTrigger("StopAbility");
            return;
        }

        //°”Àπ¥‡«≈“‡≈Ëπparticle∑—Èßµ—«∑’Ë‡ªÁπChildren
        var mainMoudle = flameThrow.main;
        var extraModule = flameThrow.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        mainMoudle.duration = flameThrowDuration;
        extraModule.duration = flameThrowDuration;

        flameThrow.Clear();
        flameThrow.Play();
    }
    public void ActivateHammer()
    {
        GameObject newActivation = Object_Pool.instance.GetObject(hammerFxPrefab, impactPoint);
        Object_Pool.instance.ReturnObject(newActivation, 1);
        MassDamage(damagePoints[0].position, hammerCheckRadius,hammerActiveDamage);
    }
    public void SetAbilityCooldown() => lastTimeUsedAbility = Time.time; //‡´Á∑§Ÿ≈Ï¥“«À≈—ß‡≈ËπÕπ‘‡¡™—Ëπ‡ √Á®
    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint;
        if (impactPoint == null)
        {
            impactPoint = transform;
        }
        MassDamage(impactPoint.position, impactRadius,jumpAttackDamage);

    }
    private void MassDamage(Vector3 impactPoint, float impactRadius,int damage) //§«∫§ÿ¡¥“‡¡®°“√‚®¡µ’¢Õß»—µ√Ÿ∑∑’Ë‡ªÁπ«ß°«È“ß
    {
        HashSet<GameObject> uniqueEntitie = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatIsAlly);
        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null) //‡™Á§«Ë“¿“¬„πæ◊Èπ∑’Ë∑’Ë°√–‚¥¥≈ß¡“obj‰Àπ¡’Idamagable∫È“ß
            {
                GameObject rootEntitie = hit.transform.root.gameObject;
                if (uniqueEntitie.Add(rootEntitie) == false)
                {
                    continue;
                }
                damagable.TakeDamage(damage);
            }

            ApplyPhysicalForce(impactPoint, impactRadius, hit);
        }

    }

    private void ApplyPhysicalForce(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        //Physic effect
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upwardsMultiplier, ForceMode.Impulse);
        }
    }

    public bool CanDoJumpAttack()
    {
        float distanceToplayer = Vector3.Distance(transform.position, player.position);
        if (distanceToplayer < minJumpDistanceRequired) //ºŸÈ‡≈ËπÕ¬ŸË„°≈È‚¥¥‰¡Ë‰ª‰¡Ë®”‡ªÁπµÈÕß°√–‚¥¥
        {
            return false;
        }
        if (Time.time > lastTimeJump + jumpAttackCooldown && PlayerInClearSight())
        {

            return true;
        }
        return false;
    }
    public void SetJumpAttackCooldown() => lastTimeJump = Time.time;

    public bool PlayerInClearSight()
    {
        Vector3 enemyPos = transform.position + new Vector3(0, 3.07f, 0); //+∫«°¥È«¬§«“¡ Ÿß¢Õß∫Õ 
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - enemyPos).normalized;
        //„ÀÈµ√«®®—∫„π√–¬–∑“ß«Ë“¡’LayerMaskÕ–‰√¢«“ßÕ¬ŸË¡—È¬·µË „ÀÈignore LayerMask∑’Ë‡ªÁπenemy
        if (Physics.Raycast(enemyPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            //∫“ß∑’‡«≈“raycast®“°∑’Ë ŸßÊ‰¡Ë‚¥πCollider¢ÕßºŸÈ‡≈Ëπ‡≈¬µÈÕß‡™Á§«Ë“parent¢Õß¡—π¡’LayerMask player¡—È¬
            if (hit.transform.root == player.root)
            {
                return true;
            }
        }
        return false;
    }
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        if (damagePoints.Length > 0)
        {
            foreach (var point in damagePoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(damagePoints[0].position, hammerCheckRadius);
        }

    }

}

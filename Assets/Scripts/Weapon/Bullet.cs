using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    public float ImpactForce;

    private Rigidbody rb;
    private TrailRenderer trailRenderer; //�Ϳ࿡�ҧ����ع
    private MeshRenderer meshRenderer;
    private BoxCollider cd;

    [SerializeField] private GameObject bulletFX;
    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;
    public void BulletSetup(float flyDistance,int bulletDamage, float imppactForce = 100)
    {
        this.ImpactForce = imppactForce;
        this.bulletDamage = bulletDamage;


        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = .25f;
        startPosition = transform.position; //transform.position��͵��˹觢ͧ�ѵ�ع��㹻Ѩ�غѹ
        this.flyDistance = flyDistance + .7f; //setupflydistance�����ҡѺgundistance+.5���١����
        //�ǡ.7����������С���ع�š���������Դ�֧


    }
    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        FadeTrailIfNeeded();

        DisableBulletIfNeeded();

        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0) //���Ңͧ�Ϳ࿤�ҧ����ع���¡���0����Ҥ������������
        {
            ReturnBulletToPool();

        }
    }

    private void ReturnBulletToPool()
        => Object_Pool.instance.ReturnObject(gameObject);
    //����Թobj��Pool�������ҹѺ�����ѧ0


    private void DisableBulletIfNeeded()
    {
        //��ҵ��˹����������е��˹觻Ѩ�غѹ�ҡ�������з�����ع�ԧ�͡��������������������Pool
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;

        }
    }

    private void FadeTrailIfNeeded()
    {
        //��͹�ж֧�����ش���¢ͧ���зҧ����ع���Ŵ������ҢͧtrailRenderer����������ع࿴������ٷ���
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        CreateImpactFX();
        ReturnBulletToPool();

        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(bulletDamage);

        
        ApplyBulletImpactToEnemy(collision);

    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * ImpactForce; //�ӹǳ�ç�ͧ����ع
            Rigidbody hitRigibody = collision.collider.attachedRigidbody;
            enemy.BulletImpact(force, collision.contacts[0].point, hitRigibody);
        }
    }

    private void CreateImpactFX()
    {


        GameObject newImpactFX = Object_Pool.instance.GetObject(bulletFX, transform);

        Object_Pool.instance.ReturnObject(newImpactFX, 1);

    }
}

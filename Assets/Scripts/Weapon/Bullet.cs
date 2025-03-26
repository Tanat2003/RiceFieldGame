using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    public float ImpactForce;

    private Rigidbody rb;
    private TrailRenderer trailRenderer; //เอฟเฟกหางกระสุน
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
        startPosition = transform.position; //transform.positionคือตำแหน่งของวัตถุนี้ในปัจจุบัน
        this.flyDistance = flyDistance + .7f; //setupflydistanceให้เท่ากับgundistance+.5ที่ถูกส่งมา
        //บวก.7เพื่อให้ระยะกระสุนไกลกว่าเลเซอร์นิดนึง


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
        if (trailRenderer.time < 0) //เวลาของเอฟเฟคหางกระสุนน้อยกว่า0แปลว่าควรหายไปได้แล้ว
        {
            ReturnBulletToPool();

        }
    }

    private void ReturnBulletToPool()
        => Object_Pool.instance.ReturnObject(gameObject);
    //รีเทรินobjไปในPoolด้วยเวลานับถอยหลัง0


    private void DisableBulletIfNeeded()
    {
        //ถ้าตำแหน่งเริ่มต้นและตำแหน่งปัจจุบันมากกว่าระยะที่กระสุนยิงออกไปเมื่อไหร่ให้เพิ่มเข้าPool
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;

        }
    }

    private void FadeTrailIfNeeded()
    {
        //ก่อนจะถึงระยะสุดท้ายของระยะทางกระสุนให้ลดค่าเวลาของtrailRendererเพื่อให้กระสุนเฟดหายไปสมูทขึ้น
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
            Vector3 force = rb.velocity.normalized * ImpactForce; //คำนวณแรงของกระสุน
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

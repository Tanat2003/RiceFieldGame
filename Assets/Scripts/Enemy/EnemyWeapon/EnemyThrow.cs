using UnityEngine;

public class EnemyThrow : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Transform player;
    private float flySpeed;
    private float rotationSpeed = 1500;
    private Vector3 direction;
    private float timer = 1;
    private int damage;
    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer > 0)
        {
            direction = player.position + Vector3.up - transform.position; //ระยะห่างระหว่างผู้เล่นกับขวาน

        }
        transform.forward = rb.velocity;

    }
    private void FixedUpdate() //fix trailweaponthrow
    {
        rb.velocity = direction.normalized * flySpeed;
        
    }
    public void EnemyThrowSetup(float flySpeed, Transform player, float timer,int damage)
    {
        this.damage = damage;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage);
        GameObject newFx = Object_Pool.instance.GetObject(impactFx, transform);
        Object_Pool.instance.ReturnObject(gameObject);
        Object_Pool.instance.ReturnObject(newFx, 1f);


    }
   

}

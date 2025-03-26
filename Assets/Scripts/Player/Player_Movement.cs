using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private Player player;
    
    private PlayerControll controls;
    private CharacterController characterController;
    private float verticalVelocity;
    private float gravityScale = 9.81f;
    private Animator animator;
    private bool isRunning;



    public Vector3 movementDirection;


    public Vector2 moveInput {  get; private set; }

    [Header("MovementInfo")]
    private float speed;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float runSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 1f;







    private void Start()
    {
        player = GetComponent<Player>();
       
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;
        AssignInputEvent();

    }
    private void Update()
    {
        if(player.health.isDead)
        {
            return;

        }
        ApplyMovement();

        // aim for player
        ApplyRotation();

        AnimatorController();
    }

    


    private void ApplyRotation()
    {


        Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        //Lerp¤×Í¡ÒÃËÒ¤èÒÃÐËÇèÒ§ÊÍ§¨Ø´àªè¹ AÍÂÙè1 BÍÂÙè10 ËÒ¨Ø´·Õè 5 (Mathf.Lerp(¨Ø´1,¨Ø´»ÅÒÂ·Ò§,¨Ø´·ÕèµéÍ§¡ÒÃËÒÃÐÂÐ)
        //Quaternion.Slerp(¨Ø´1,¨Ø´»ÅÒÂ·Ò§,¤ÇÒÁàÃçÇ·ÕèàÃÒµéÍ§¡ÒÃà¾ÔèÁã¹ÃÐÂÐ)
        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection); //Quaternionà»ç¹µÑÇá»Ã»ÃÐàÀ·Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);

    }

    private void ApplyMovement()
    {
        
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
            




        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;


        }
        else
        {
            verticalVelocity = -.5f;

        }

    }

    private void AnimatorController()
    {

        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right); //á¡¹xã¹blendTree 
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward); //á¡¹yã¹blendtree

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime); // à«ç·¤èÒfloat xVelocityã¹blendTreeãËéà·èÒ¡ÑºxVelocity·ÕèàÃÒÊÃéÒ§ã¹¿Ñ§ªÑè¹¹Õé
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);  //¼ÙéàÅè¹à´Ô¹ã¹àà¹Çá¡¹x¤×Í«éÒÂ¢ÇÒ á¡¹z¤×ÍË¹éÒËÅÑ§ YäÁèãªéà¾ÃÒÐäÁèÁÕ¡ÃÐâ´´     

        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("IsRunning", playRunAnimation);
    }
    private void AssignInputEvent()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;



        controls.Character.Run.performed += context =>//¡´shiftáÅéÇãËéisRunningã¹Ê¤ÃÔ»¹Õéà»ç¹trueÅÐÊè§¤èÒä»
        {

            speed = runSpeed;
            isRunning = true;


        };
        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;



        };
    }











}

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

        //Lerp��͡���Ҥ�������ҧ�ͧ�ش�� A����1 B����10 �Ҩش��� 5 (Mathf.Lerp(�ش1,�ش���·ҧ,�ش����ͧ���������)
        //Quaternion.Slerp(�ش1,�ش���·ҧ,�������Ƿ����ҵ�ͧ������������)
        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection); //Quaternion�繵���û�����Rotation
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

        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right); //᡹x�blendTree 
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward); //᡹y�blendtree

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime); // �緤��float xVelocity�blendTree�����ҡѺxVelocity���������ҧ㹿ѧ��蹹��
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);  //�������Թ����᡹x��ͫ��¢�� ᡹z���˹����ѧ Y�������������ա��ⴴ     

        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("IsRunning", playRunAnimation);
    }
    private void AssignInputEvent()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;



        controls.Character.Run.performed += context =>//��shift�������isRunning�ʤ�Ի�����true���觤���
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

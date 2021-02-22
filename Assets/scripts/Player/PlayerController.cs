using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

#region Variables
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float crouchSpeed = 10f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float wallDetectionDistance = 1;
    [SerializeField] private LayerMask whatIsEnv;
    private float speedSmoothTime = 0.1f;
    public bool canMove;
    public Transform mainCameraTransform = null;
    public ParticleSystem dust;
    
    [Header("Jumping Variables")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private float doubleJumpForce = 3f;
    private bool canJumpWithoutBeingOnTheGround = true;
    private bool jumpWasPressed = false;

    [Header("State boolions")]
    public bool crouching;
    public bool sprinting;

    [HideInInspector] public Vector3 moveDir;

    [Header("Slopes")]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    private bool isJumping;

    [Header("Wallrun mechanic")]
    public float wallRunRayDistance = 2;
    public LayerMask whatIsWall;
    public float wallrunSpeed = 20;
    public float wallJumpoffForce = 50;
    public Vector3 calculatedRunningDirection;
    bool runnableWallLeft, runnableWallRight;
    bool isWallrunning;
    Vector3 wallNormalDirection;
    Transform body;

    // [Header("Pause Menu Pose")]
    // public Transform ctvPosition;
    // public float lookAtCtv = 0f;
    // public float lookAtCtvRot = 0f;

    //Stuff
    [HideInInspector] public CharacterController controller =  null;
    private PlayerCombat combat = null;
    private PlayerPhysics pphysics = null;
    
    //Animator
    private Animator anim = null;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

#endregion

#region Private Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        pphysics = GetComponent<PlayerPhysics>();
        combat = GetComponent<PlayerCombat>();
        anim = GetComponent<Animator>();
        body = transform.Find("GFX");
        
        speed = sprintSpeed;
    }
    
    private void Update() {
        DontRunWhenWall();
        HandleAnimations();
        if(canMove) {
            Movement();
            Jumping();
            CheckForWalls();
            WallrunInput();
        }

        if(Input.GetKeyDown(KeyCode.F)) {
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
        }
    }

    private void Movement() {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;
        moveDir = desiredMoveDirection;
        
        //Crouching
        if(Input.GetKey(KeyCode.C) && pphysics.IsGrounded() && movementInput != Vector2.zero) {
            crouching = true;
            speed = crouchSpeed;
            anim.SetBool("crouching", true);
        }
        
        if(Input.GetKeyUp(KeyCode.C)) {
            crouching = false;
            speed = sprintSpeed;
            anim.SetBool("crouching", false);
        }

        //Moving state
        if(movementInput != Vector2.zero) {
            sprinting = true;
        } else {
            sprinting = false;
        }

        if(movementInput != Vector2.zero && pphysics.IsGrounded()) {
            CreateDust();
        }

        //Rotate player and set speed
        if(!isWallrunning) {
            if(desiredMoveDirection != Vector3.zero && combat.targets.Count <= 0) 
            {
                if(transform.Find("CurrentAbilities").Find("Shield(Clone)") != null) {
                    if(!transform.Find("CurrentAbilities").Find("Shield(Clone)").gameObject.GetComponent<ShieldAbility>().blocking)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);
                } else {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);
                }
            }
        }
        
        float currentSpeed = 0;
        float targetSpeed = speed * movementInput.magnitude;
        currentSpeed = Mathf.Lerp(0, targetSpeed, speedSmoothTime);

        //Actually move
        if(!isWallrunning) {
            controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
        }

        if ((movementInput.y != 0 || movementInput.x != 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        
        anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;
        return false;
    }

    private void WallrunInput() 
    {
        if(runnableWallRight || runnableWallLeft)
            if(Input.GetKey(KeyCode.W)) {
                StartWallRun();
            }
        anim.SetBool("isWallrunning", isWallrunning);
    }

    private void StartWallRun()
    {
        isWallrunning = true;
        pphysics.useGravity = false;

        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(calculatedRunningDirection), 0.2f);
        controller.Move(calculatedRunningDirection * wallrunSpeed * Time.deltaTime);

        if(runnableWallRight)
            pphysics.AddImpact(transform.right, wallrunSpeed / 5 * Time.deltaTime);
        else
            pphysics.AddImpact(-transform.right, wallrunSpeed / 5 * Time.deltaTime);
    }

    private void StopWallRun()
    {
        pphysics.useGravity = true;
        isWallrunning = false;
    }

    void CheckForWalls()
    {
        RaycastHit right, left;

        Physics.Raycast(transform.position, transform.right, out right, wallRunRayDistance, whatIsWall);
        Physics.Raycast(transform.position, -transform.right, out left, wallRunRayDistance, whatIsWall);
        
        if(right.collider != null) {
            runnableWallRight = Vector3.Dot(right.normal, Vector3.up) == 0;
            calculatedRunningDirection = Vector3.Cross(right.normal, Vector3.up) * -1;
            wallNormalDirection = right.normal;
        } else 
            runnableWallRight = false;

        if(left.collider != null) {
            runnableWallLeft = Vector3.Dot(left.normal, Vector3.up) == 0;
            calculatedRunningDirection = Vector3.Cross(left.normal, Vector3.up);
            wallNormalDirection = left.normal;
        } else 
            runnableWallLeft = false;

        if(!runnableWallLeft && !runnableWallRight) StopWallRun();
        if(runnableWallLeft || runnableWallRight)
            canDoubleJump = true;
    }

    private void Jumping() {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            jumpWasPressed = true;
            StartCoroutine(RememberJumpTime());
            if(canJumpWithoutBeingOnTheGround)
            {
                pphysics.velocity.y = Mathf.Sqrt(jumpForce * -2f * pphysics.gravity);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump && !canJumpWithoutBeingOnTheGround) 
        {
            canDoubleJump = false;
            pphysics.velocity.y = Mathf.Sqrt(doubleJumpForce * -2f * pphysics.gravity);
        }
        
        if(!pphysics.IsGrounded())
        {
            StartCoroutine(CoyoteTime());
        }

        if(pphysics.IsGrounded())
        {
            canDoubleJump = true;
            canJumpWithoutBeingOnTheGround = true;
            if(jumpWasPressed)
            {
                pphysics.velocity.y = Mathf.Sqrt(jumpForce * -2f * pphysics.gravity);
            }
        }  

        if(isWallrunning)
        {
            if(Input.GetKeyDown(KeyCode.Space)) {
                StopWallRun();
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(wallNormalDirection), 0.2f);
                pphysics.AddImpact(wallNormalDirection + Vector3.up, wallJumpoffForce);
            }
        }  
    }

    private IEnumerator CoyoteTime() {
        yield return new WaitForSeconds(.1f);
        canJumpWithoutBeingOnTheGround = false;
    }

    private IEnumerator RememberJumpTime() {
        yield return new WaitForSeconds(.1f);
        jumpWasPressed = false;
    }

    private void HandleAnimations() {
        if (pphysics.IsGrounded() == false) {
            anim.SetBool("isGrounded", false);

            anim.SetFloat("velocityY", pphysics.velocity.y);
        }
        if(pphysics.IsGrounded()) {
            anim.SetBool("isGrounded", true);
            anim.SetFloat("velocityY", 0);
        }
    }
    
    private void DontRunWhenWall() {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        if(IsFacingAWall())
            anim.SetFloat(hashSpeedPercentage, 0.4f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
    }
    
    private bool IsFacingAWall() {
        return Physics.Raycast(transform.position, transform.forward, wallDetectionDistance, whatIsEnv);
    }

    void CreateDust() {
        dust.Play();
    }
#endregion
}
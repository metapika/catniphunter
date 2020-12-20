using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

#region Variables
    public bool lockCursor;
    [SerializeField] private bool enableDoubleJump = true;
    
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float wallDetectionDistance = 1;
    [SerializeField] private LayerMask whatIsEnv;
    private float speedSmoothTime = 0.1f;
    public bool canMove = true;
    
    [Header("Jumping Variables")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private float doubleJumpForce = 3f;

    [Header("State boolions")]
    public bool walking;
    public bool crouching;
    public bool sprinting;

    [HideInInspector] public Vector3 moveDir;

    [Header("Slopes")]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    private bool isJumping;

    [Header("Wallbounce")]
    public Transform orientation;
    public LayerMask whatIsWall;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float wallRayDetect = 10f;
    public bool isWallRight, isWallLeft;

    //Stuff
    private CharacterController controller =  null;
    private PlayerPhysics pphysics = null;
    private Transform mainCameraTransform = null;
    
    //Animator
    private Animator anim = null;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

#endregion

#region Private Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        pphysics = GetComponent<PlayerPhysics>();
        anim = GetComponent<Animator>();
        mainCameraTransform = Camera.main.transform;
    }
    
    private void Update() {
        DontRunWhenWall();
        CheckWall();
        HandleAnimations();
        if(canMove) {
            Movement();
            Jumping();
        }

        //Lock cursor
        if(lockCursor == true)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
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
        
        //Walking
        if(Input.GetKey(KeyCode.LeftControl) && pphysics.IsGrounded()) {
            walking = true;
            anim.SetFloat(hashSpeedPercentage, 0.5f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        } else {
            walking = false;
            if(!IsFacingAWall())
            anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        }

        //Crouching
        if(Input.GetKey(KeyCode.C) && pphysics.IsGrounded()) {
            crouching = true;
            anim.SetBool("crouching", true);
        } else {
            crouching = false;
            anim.SetBool("crouching", false);
        }

        //Moving state
        if(movementInput != Vector2.zero) {
            sprinting = true;
        } else {
            sprinting = false;
        }
        if(walking || crouching || IsFacingAWall()) {
            speed = walkSpeed;
        } else if(sprinting) {
            speed = sprintSpeed;
        }

        //Rotate player and set speed
        if(desiredMoveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);
        }
        
        float currentSpeed = 0;
        float targetSpeed = speed * movementInput.magnitude;
        currentSpeed = Mathf.Lerp(0, targetSpeed, speedSmoothTime);

        //Actually move
        controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);

        if ((movementInput.y != 0 || movementInput.x != 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
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
    
    private void Jumping() {       
        if(pphysics.IsGrounded() && !walking) {
            
            canDoubleJump = true;
            if(Input.GetKeyDown(KeyCode.Space)) {
                //Jump
                pphysics.velocity.y = Mathf.Sqrt(jumpForce * -2f * pphysics.gravity);
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump && enableDoubleJump && !isWallRight && !isWallLeft) {
                //Jump
                pphysics.velocity.y = Mathf.Sqrt(doubleJumpForce * -2f * pphysics.gravity);
                canDoubleJump = false;
            }
        }
    }
    
    void CheckWall() {
        isWallRight = Physics.Raycast(transform.position, orientation.right, wallRayDetect, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, wallRayDetect, whatIsWall);
    }

    void WallBounce() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(isWallRight && !pphysics.IsGrounded()) {
                pphysics.velocity += orientation.right * Mathf.Sqrt(bounceForce * -2f * pphysics.gravity);;
            } else if(isWallLeft && !pphysics.IsGrounded()) {
                pphysics.velocity += -orientation.right * Mathf.Sqrt(bounceForce * -2f * pphysics.gravity);;
            }
        }
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
        return Physics.Raycast(transform.position, transform.forward, wallDetectionDistance);
    }
#endregion
}
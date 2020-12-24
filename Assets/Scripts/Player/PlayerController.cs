using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

#region Variables
    [SerializeField] private bool enableDoubleJump = true;
    
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float crouchSpeed = 10f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float wallDetectionDistance = 1;
    [SerializeField] private LayerMask whatIsEnv;
    private float speedSmoothTime = 0.1f;
    public bool canMove;
    public Transform mainCameraTransform = null;
    
    [Header("Jumping Variables")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private float doubleJumpForce = 3f;

    [Header("State boolions")]
    public bool crouching;
    public bool sprinting;

    [HideInInspector] public Vector3 moveDir;

    [Header("Slopes")]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    private bool isJumping;

    //Stuff
    private CharacterController controller =  null;
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
        
        speed = sprintSpeed;
    }
    
    private void Update() {
        DontRunWhenWall();
        HandleAnimations();
        if(canMove) {
            Movement();
            Jumping();
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
        
        if(Input.GetKeyUp(KeyCode.C) || movementInput == Vector2.zero) {
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
    
    private void Jumping() {       
        if(pphysics.IsGrounded() && !crouching) {
            
            canDoubleJump = true;
            if(Input.GetKeyDown(KeyCode.Space)) {
                //Jump
                pphysics.velocity.y = Mathf.Sqrt(jumpForce * -2f * pphysics.gravity);
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump && enableDoubleJump) {
                //Jump
                pphysics.velocity.y = Mathf.Sqrt(doubleJumpForce * -2f * pphysics.gravity);
                canDoubleJump = false;
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
        return Physics.Raycast(transform.position, transform.forward, wallDetectionDistance, whatIsEnv);
    }
#endregion
}
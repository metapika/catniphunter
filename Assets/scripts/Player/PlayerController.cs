using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] private LayerMask whatIsEnv;
    [SerializeField] private float slopeForce = 13f;
    [SerializeField] private float slopeForceRayLength = 1.2f;
    private float originalHeight;
    [SerializeField] private Vector3 originalCenter;

    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public CharacterController controller;
    public bool canRotate = true;
    public bool canMove = true;
    public bool crouching = false;
    private float speedSmoothTime = 0.1f;
    private bool jumpWasPressed;
    private bool canJumpNoGrounded;
    private bool canDoubleJump;
    private PlayerStats stats;
    private PlayerPhysics pphysics;
    private PlayerCombat combat;
    private Animator anim;
    private float stepOffset;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");
    public MoneyManager money;
    
    #endregion

    #region Unity Functions
    void Awake() {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        pphysics = GetComponent<PlayerPhysics>();
        combat = GetComponent<PlayerCombat>();
        anim = GetComponent<Animator>();

        originalHeight = controller.height;
        originalCenter = controller.center;
        stepOffset = controller.stepOffset;

        money = MoneyManager.singleton;
    }

    void Update() {
        Movement();
        Jumping();
        HandleAnimations();

        if(Input.GetKeyDown(KeyCode.L)) {
            money.AddMoney(100);
        } else if(Input.GetKeyDown(KeyCode.K)) {
            money.RemoveMoney(100);
        }

        pphysics.controllerVelocity.x = controller.velocity.x;
        pphysics.controllerVelocity.z = controller.velocity.z;
    }

    #endregion

    #region Custom Functions
    private void Movement() {
        if(!canMove) return;

        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;
        moveDir = desiredMoveDirection;

        if(pphysics.IsGrounded()) {
            if(Input.GetButtonDown("Crouch")) {
                if(crouching)
                {
                    UnCrouch();

                }
                else
                {
                    Crouch();
                }
            }
        }
        if(pphysics.IsGrounded()) {
            controller.stepOffset = stepOffset;
        } else if(!pphysics.sliding) {
            controller.stepOffset = 0;
        }

        float calculatedSpeed = 0;
        float targetSpeed = stats.currentSpeed * movementInput.magnitude;
        calculatedSpeed = Mathf.Lerp(0, targetSpeed, speedSmoothTime);
        if(Time.timeScale > 0) {
            if(canRotate && desiredMoveDirection != Vector3.zero) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);
            }
        }
        
        controller.Move(desiredMoveDirection * calculatedSpeed * Time.deltaTime);


        if ((movementInput.y != 0 || movementInput.x != 0) && OnSlope()) {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
    }
    public void UnCrouch()
    {
        Ray crouchRay = new Ray(transform.position + Vector3.up * controller.radius * 0.5f, Vector3.up);
        float crouchRayLength = originalHeight - controller.radius * 0.5f;
        if (CheckIfCanUncrouch())
        {
            crouching = true;
            return;
        }
        controller.height = originalHeight;
        controller.center = originalCenter;

        crouching = false;

        stats.ChangeSpeed(stats.sprintSpeed);
        anim.SetBool("crouching", false);
    }
    public bool CheckIfCanUncrouch()
    {
        Ray crouchRay = new Ray(transform.position + Vector3.up * controller.radius * 0.5f, Vector3.up);
        float crouchRayLength = originalHeight - controller.radius * 0.5f;
        return Physics.SphereCast(crouchRay, controller.radius * 0.5f, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);
    }
    private void Crouch()
    {
        crouching = true;
        controller.height = 1.3f;
        controller.center = new Vector3(0f, -0.15f, 0f);
        stats.ChangeSpeed(stats.crouchSpeed);
        anim.SetBool("crouching", true);
    }
    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength)) {
            if (hit.normal != Vector3.up) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    private void Jumping() {
        if(!canMove) return;
        
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if(crouching) {UnCrouch(); return;}

            jumpWasPressed = true;
            StartCoroutine(RememberJumpTime());
            if(canJumpNoGrounded)
            {
                pphysics.velocity.y = Mathf.Sqrt(stats.jumpForce * -2f * pphysics.currentGravity);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump && !canJumpNoGrounded) 
        {
            canDoubleJump = false;
            pphysics.velocity.y = Mathf.Sqrt(stats.doubleJumpForce * -2f * pphysics.currentGravity);
        }
        
        if(!pphysics.IsGrounded())
        {
            StartCoroutine(CoyoteTime());
        }

        if(pphysics.IsGrounded())
        {
            canDoubleJump = true;
            canJumpNoGrounded = true;
            if(jumpWasPressed)
            {
                pphysics.velocity.y = Mathf.Sqrt(stats.jumpForce * -2f * pphysics.currentGravity);
            }
        }  
    }

    private IEnumerator CoyoteTime() {
        yield return new WaitForSeconds(.1f);
        canJumpNoGrounded = false;
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
    #endregion
}

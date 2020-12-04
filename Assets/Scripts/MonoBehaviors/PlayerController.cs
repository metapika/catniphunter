using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

#region Variables
    public bool lockCursor;
    
    //Movement and jumping
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpRaycastDistance = 1f;
    private float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity = 0f;
    private CharacterController controller =  null;
    private Transform mainCameraTransform = null;
    
    //State bools
    public bool walking, crouching, sprinting, standingInPlace;

    //Velocity and gravity
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    //Sliding && dashing
    public float dashSpeed;
    public float dashTime;
    private Vector3 moveDir;

    //Cooldowns
    private float dashCooldownTime = 2;
    private float dashNextFireTime = 0;

    //Animator
    private Animator anim = null;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

#endregion

#region Private Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        mainCameraTransform = Camera.main.transform;
    }
    
    private void Update() {
        Jump();
        Movement();
        HandleAnimations();

        //Dash cooldown
        if(Time.time > dashNextFireTime)
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded() && sprinting)
            {
                print("dash used");
                dashNextFireTime = Time.time + dashCooldownTime;
                StartCoroutine(Dash());
            }
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
        
        float currentSpeed = 0;

        
        //Reset gravity
        if(IsGrounded() && velocity.y < 0)
        {
            velocity.y = 0f;
        }
       
        //Walking
        if(Input.GetKey(KeyCode.LeftControl) && IsGrounded()) {
            walking = true;
            anim.SetFloat(hashSpeedPercentage, 0.5f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        } else {
            walking = false;
            anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        }

        //Crouching
        if(Input.GetKey(KeyCode.C) && IsGrounded()) {
            crouching = true;
            anim.SetBool("crouching", true);
        } else {
            crouching = false;
            anim.SetBool("crouching", false);
        }

        if(movementInput != Vector2.zero) {
            sprinting = true;
            standingInPlace = false;
        } else {
            standingInPlace = true;
            sprinting = false;
        }

        //Moving state
        if(walking || crouching) {
            speed = walkSpeed;
        } else if(sprinting) {
            speed = sprintSpeed;
        }

        //Rotate player and set speed
        if(desiredMoveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.1f);
        }
        float targetSpeed = speed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        //Actually move
        controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
    
    private void Jump() {       
        if(IsGrounded() && !walking) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                //Jump
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
    }

    private IEnumerator Dash() {
        
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(moveDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void HandleAnimations() {
        if (IsGrounded() == false) {
            anim.SetBool("isGrounded", false);

            anim.SetFloat("velocityY", velocity.y);
        }
        if(IsGrounded()) {
            anim.SetBool("isGrounded", true);
            anim.SetFloat("velocityY", 0);
        }
    }
    
    private bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, jumpRaycastDistance);
    }

#endregion
}
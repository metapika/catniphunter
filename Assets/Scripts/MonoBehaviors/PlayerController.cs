using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

#region Variables
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpRaycastDistance = 1f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 velocity;
    private float speedSmoothTime = 0.1f;
    private Animator anim = null;
    private CharacterController controller =  null;
    private Transform mainCameraTransform = null;
    private float speedSmoothVelocity = 0f;

    //Sliding && dashing
    public float dashSpeed;
    public float dashTime;
    public Vector3 moveDir;

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

        if(Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
        {
            StartCoroutine(Dash());
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
        float currentSpeed = 0;

        //Rotate player and set speed
        if(desiredMoveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.1f);
        }
        float targetSpeed = speed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        //Reset gravity
        if(IsGrounded() && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        moveDir = desiredMoveDirection;
        //Walking
        if(Input.GetKey(KeyCode.LeftControl) && IsGrounded()) {
            speed = walkSpeed;
            anim.SetFloat(hashSpeedPercentage, 0.5f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        } else {
            speed = sprintSpeed;
            anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
        }
        
        //Sprinting and walking
        controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
    
    private void Jump() {       
        if(IsGrounded()) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
    }

    private IEnumerator Dash() {
        
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            anim.SetBool("Dash", true);
            controller.Move(moveDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
        anim.SetBool("Dash", false);
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
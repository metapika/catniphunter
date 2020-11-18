using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region A lot of variables
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float speedSmoothTime = 0.1f;
    [SerializeField] private float jumpRaycastDistance = 1f;

    [SerializeField] private Vector3 velocity;
    [SerializeField] private float gravity = -9.81f;

    private Animator anim = null;
    private CharacterController controller =  null;
    private Transform mainCameraTransform = null;
    private float speedSmoothVelocity = 0f;
    private float currentSpeed = 0f;
    CharacterStats stats;

    #endregion

    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Jump();
        Move();
        HandleAnimations();
    }

    private void Move()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
        
        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x).normalized;

        if(desiredMoveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.1f);
        }
        
        float targetSpeed = speed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        
        if(IsGrounded() && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        
        controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        anim.SetFloat(hashSpeedPercentage, 1f * movementInput.magnitude, speedSmoothTime, Time.deltaTime);
    }
    
    private void Jump()
    {       
        if(IsGrounded())
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
    }

    private void HandleAnimations()
    {
        if (IsGrounded() == false)
        {
            anim.SetBool("isGrounded", false);

            anim.SetFloat("velocityY", velocity.y);
        }
        if(IsGrounded())
        {
            anim.SetBool("isGrounded", true);
            anim.SetFloat("velocityY", 0);
        }
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, jumpRaycastDistance);
    }
}
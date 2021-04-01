using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    #region Fields
    public Vector3 controllerVelocity;
    public bool useGravity = true;
    public float gravity = -39.24f;
    public float currentGravity;
    public float pushPower = 5f;
    [SerializeField] private int gravityScale = 1;
    [SerializeField] private float jumpRaycastDistance = 1.2f;
    [SerializeField] private LayerMask whatIsGround;
    [HideInInspector] public Vector3 velocity;

    private float mass = 1f;
    private Vector3 impact = Vector3.zero;
    public Transform activePlatform;
    Vector3 moveDirection;
    Vector3 activeGlobalPlatformPoint;
    Vector3 activeLocalPlatformPoint;
    Quaternion activeGlobalPlatformRotation;
    Quaternion activeLocalPlatformRotation;

    private CharacterController controller;
    private PlayerController movement;

    #endregion

    #region Unity Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerController>();

        HandleGravityChange();
    }
    Vector3 connectionWorldPosition;
    private void Update() {
        if(useGravity) {
            if(!IsGrounded())
            {
                velocity.y += gravity * Time.deltaTime;
            }
            controller.Move(velocity * Time.deltaTime);
        }

        if (impact.magnitude > 0.2) controller.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        controllerVelocity.y = controller.velocity.y;

        if (activePlatform != null)
        {
            Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
            moveDirection = newGlobalPlatformPoint - activeGlobalPlatformPoint;
            if (moveDirection.magnitude > 0.01f)
            {
                controller.Move(moveDirection);
            }
            if (activePlatform)
            {
                // Support moving platform rotation
                Quaternion newGlobalPlatformRotation = activePlatform.rotation * activeLocalPlatformRotation;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(activeGlobalPlatformRotation);
                // Prevent rotation of the local up vector
                rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                transform.rotation = rotationDiff * transform.rotation;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                UpdateMovingPlatform();
            }
        }
        else
        {
            if (moveDirection.magnitude > 0.01f)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime);
                controller.Move(moveDirection);
            }
        }

        if(!IsGrounded())
        {
            activePlatform = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // if(velocity.y < 0) {
        //     if(hit.gameObject.layer == 8 && velocity.y < 0)
        //     {
        //         velocity.x = 0f;
        //         velocity.z = 0f;
        //     }
        //     if(IsGrounded()) {
        //         velocity.y = 0f;
        //     }
        // }

        if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41)
        {
            if (activePlatform != hit.collider.transform)
            {
                if(!hit.collider.transform.CompareTag("Enemy")) {
                    activePlatform = hit.collider.transform;
                    UpdateMovingPlatform();
                }
            }
        }
        else
        {
            activePlatform = null;
        }

        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

    #endregion

    #region Custom Functions
    public void AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
            impact += dir.normalized * force / mass;
    }
    void UpdateMovingPlatform()
    {
        activeGlobalPlatformPoint = transform.position;
        activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);

        activeGlobalPlatformRotation = transform.rotation;
        activeLocalPlatformRotation = Quaternion.Inverse(activePlatform.rotation) * transform.rotation;
    }
    public void HandleGravityChange() {
        currentGravity = gravity * gravityScale;
    }
    public bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, jumpRaycastDistance, whatIsGround);
    }

    #endregion
}

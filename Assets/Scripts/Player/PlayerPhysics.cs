using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {
    [Header("Moving platform target")]
    public Transform activePlatform;
    public bool isOnPlatform = false;

    [Header("Rigidbody interactions")]
    public float pushPower = 2.0f;

    [Header("Gravity and forces")]
    public bool useGravity = true;
    public float gravity = -39.24f;
    private float gravityValue;
    [HideInInspector] public Vector3 velocity;
    [SerializeField] private float jumpRaycastDistance = 1f;

    CharacterController controller;
    PlayerController player;
    Vector3 moveDirection;
    Vector3 activeGlobalPlatformPoint;
    Vector3 activeLocalPlatformPoint;
    Quaternion activeGlobalPlatformRotation;
    Quaternion activeLocalPlatformRotation;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GetComponent<PlayerController>();

        gravityValue = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        if(useGravity == false)
            gravity = 0f;
        else
            gravity = gravityValue;
        
        if (activePlatform != null)
        {
            Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
            moveDirection = newGlobalPlatformPoint - activeGlobalPlatformPoint;
            Vector3 distanceFromPlatform = transform.position - activePlatform.position;
            if (moveDirection.magnitude > 0.01f)
            {
                if(isOnPlatform == true) {
                    controller.Move(moveDirection);
                }
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
                if(isOnPlatform == true) {
                    moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime);
                    controller.Move(moveDirection);
                }
            }
        }
        
        //Gravity and force
        if(!IsGrounded())
            velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Reset gravity
        if(hit.gameObject.layer == 8 && velocity.y < 0)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }
        if(IsGrounded() && velocity.y < 0) {
            velocity.y = 0f;
        }

        // Make sure we are really standing on a straight platform *NEW*
        // Not on the underside of one and not falling down from it either!
        if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41 && hit.transform.gameObject.CompareTag("MovingPlatform"))
        {
            if (activePlatform != hit.collider.transform)
            {
                isOnPlatform = true;
                activePlatform = hit.collider.transform;
                UpdateMovingPlatform();
            }
        } else {
            activePlatform = null;
            isOnPlatform = false;
        }
        
        //////////////////////////////////////////////
        //RIGIDBODY INTERACTION
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
    
    void UpdateMovingPlatform()
    {
        activeGlobalPlatformPoint = transform.position;
        activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
        // Support moving platform rotation
        activeGlobalPlatformRotation = transform.rotation;
        activeLocalPlatformRotation = Quaternion.Inverse(activePlatform.rotation) * transform.rotation;
    }
    
    public bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, jumpRaycastDistance);
    }
}
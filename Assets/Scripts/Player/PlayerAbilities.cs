using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Enable abilities")]
    public bool enableAirDash = true;
    public bool enableShield = true;
    
    //Controllers and other components
    private PlayerController player;
    private PlayerPhysics pphysics;
    private CharacterController controller;
    private Animator anim;

    [Space]

    [Header("Mid-air dash Variables")]
    [SerializeField] private float airDashSpeed = 20f;
    [SerializeField] private float airDashTime = 1f;
    [SerializeField] private bool canAirDash = false;

    [Space]

    [Header("Shield Variables")]
    public GameObject shield;
    public bool blocking;
    public GameObject target;

#region Private functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        pphysics = GetComponent<PlayerPhysics>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update() {
        //Mid-air dash
        if(pphysics.IsGrounded() && enableAirDash)
            canAirDash = true;
        
        if(enableAirDash && Input.GetKeyDown(KeyCode.LeftShift) && !pphysics.IsGrounded() && player.sprinting && canAirDash) {
            anim.SetBool("dashing", true);
            StartCoroutine(AirDash());
        }

        //Shield
        if(enableShield) {
            if(Input.GetButton("Fire2") && pphysics.IsGrounded())
                Shield();
            if(Input.GetButtonUp("Fire2"))
                DisableShield();
        }
    }
    
    private IEnumerator AirDash() {
        
        float startTime = Time.time;

        while(Time.time < startTime + airDashTime)
        {
            canAirDash = false;
            controller.Move(player.moveDir * airDashSpeed * Time.deltaTime);
            yield return null;
        }
        anim.SetBool("dashing", false);
    }

    private void Shield() {
        anim.SetBool("blocking", true);
        if(shield != null)
            shield.SetActive(true);
        blocking = true;
        
        Vector3 forward = target.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        transform.LookAt(transform.position + forward);
        player.canMove = false;
    }

    private void DisableShield() {
        anim.SetBool("blocking", false);
        if(shield != null)
            shield.SetActive(false);
        blocking = false;
        player.canMove = true;
    }
#endregion
}

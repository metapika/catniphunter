using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Enable abilities")]
    public bool enableDash = true;
    public bool enableAirDash = true;
    
    //Controllers and other components
    private PlayerController player;
    private PlayerPhysics pphysics;
    private CharacterController controller;
    private Animator anim;

    [Header("Dash Variables")]
    //Sliding && dashing cooldowns
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.25f;
    [SerializeField] private float dashCooldownTime = 2f;
    private float dashNextFireTime = 0f;

    [Header("Mid-air dash Variables")]
    [SerializeField] private float airDashSpeed = 20f;
    [SerializeField] private float airDashTime = 1f;
    [SerializeField] private bool canAirDash = false;

#region Private functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        pphysics = GetComponent<PlayerPhysics>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update() {
        //Dash
        if(enableDash) {
            if(Time.time > dashNextFireTime)
            {
                if(Input.GetKeyDown(KeyCode.LeftShift) && pphysics.IsGrounded() && player.sprinting)
                {
                    dashNextFireTime = Time.time + dashCooldownTime;
                    StartCoroutine(Dash());
                }
            }
        }

        //Mid-air dash
        if(pphysics.IsGrounded() && enableAirDash)
            canAirDash = true;
        
        if(enableAirDash && Input.GetKeyDown(KeyCode.LeftShift) && !pphysics.IsGrounded() && player.sprinting && canAirDash) {
            StartCoroutine(AirDash());
        }
    }

    private IEnumerator Dash() {
        
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(player.moveDir * dashSpeed * Time.deltaTime);
            yield return null;
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
    }
#endregion
}

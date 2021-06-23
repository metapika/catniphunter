using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShieldAbility : MonoBehaviour
{
    public GameObject shield;
    private Shield shieldBehavior;
    public bool blocking;
    private Animator playerAnim;
    private Animator shieldAnim;
    private PlayerPhysics pphysics;
    private PlayerStats stats;
    private PlayerCombat combat;
    public BoxCollider col;

    private void Awake() {
        pphysics = transform.root.gameObject.GetComponent<PlayerPhysics>();
        combat = transform.root.gameObject.GetComponent<PlayerCombat>();
        stats = transform.root.gameObject.GetComponent<PlayerStats>();
        playerAnim = transform.root.gameObject.GetComponent<Animator>();
        shieldBehavior = shield.GetComponent<Shield>();
        shieldAnim = shield.GetComponent<Animator>();

        shieldBehavior.gfx.SetActive(false);
    }

    private void Update() {

        if(combat.currentWeapon)
        {
            if(Input.GetButton("Shield") && pphysics.IsGrounded() && combat.currentWeapon.canAttack)
            {
                Shield();
            }
            if(Input.GetButtonUp("Shield") || !combat.currentWeapon.canAttack)
            {
                DisableShield();
            }
        } 
        else {
            if(Input.GetButton("Shield") && pphysics.IsGrounded())
            {
                Shield();
            } else if(Input.GetButtonUp("Shield"))
            {
                DisableShield();
            }
        }
        if(blocking)
        {
            playerAnim.SetBool("blocking", true);
            if(stats.controller.moveDir == Vector3.zero)
            {
                playerAnim.SetBool("blockingStanding", true);
            } else {
                playerAnim.SetBool("blockingStanding", false);
            }
            
            stats.controller.canRotate = false;
            stats.controller.canJump = false;
            
            if(!combat.camControl.CameraToggleState())
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();
                
                transform.root.LookAt(transform.root.position + forward);
            }
        }
        else
        {
            playerAnim.SetBool("blocking", false);
            playerAnim.SetBool("blockingStanding", false);

            if(!combat.camControl.CameraToggleState())
            stats.controller.canRotate = true;
            stats.controller.canJump = true;
        }
    }
    private void Shield() {
        shieldAnim.SetBool("blocking", true);

        stats.ChangeSpeed(stats.GetShieldSpeed());

        shieldBehavior.gfx.SetActive(true);
        col.enabled = true;
        blocking = true;
    }

    public void DisableShield() {
        shieldAnim.SetBool("blocking", false);
        if(stats.controller.crouching) stats.ChangeSpeed(stats.GetCrouchSpeed());
        else stats.ChangeSpeed(stats.GetSprintSpeed());

        col.enabled = false;
        blocking = false;
    }
}

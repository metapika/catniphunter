using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShieldAbility : MonoBehaviour
{
    public GameObject shield;
    private Shield shieldBehavior;
    public bool blocking;
    private PlayerPhysics pphysics;
    private Animator playerAnim;
    private Animator shieldAnim;
    private PlayerController player;
    private PlayerCombat combat;
    public BoxCollider col;

    private void Awake() {
        pphysics = transform.root.gameObject.GetComponent<PlayerPhysics>();
        player = transform.root.gameObject.GetComponent<PlayerController>();
        combat = transform.root.gameObject.GetComponent<PlayerCombat>();
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

    }
    private void Shield() {
        playerAnim.SetBool("blocking", true);
        shieldAnim.SetBool("blocking", true);

        shieldBehavior.gfx.SetActive(true);
        col.enabled = true;
        blocking = true;
        if(combat.targets.Count == 0)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            transform.root.LookAt(transform.root.position + forward);
        }
    }

    public void DisableShield() {
        playerAnim.SetBool("blocking", false);
        shieldAnim.SetBool("blocking", false);
        col.enabled = false;
        blocking = false;
    }
}

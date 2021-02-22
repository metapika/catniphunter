using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShieldAbility : MonoBehaviour
{
    [Header("Shield Variables")]
    public GameObject shield;
    private Shield shieldBehavior;
    public bool blocking;
    private PlayerPhysics pphysics;
    private Animator playerAnim;
    private Animator shieldAnim;
    private PlayerController player;
    public BoxCollider col;

    private void Awake() {
        pphysics = transform.root.gameObject.GetComponent<PlayerPhysics>();
        player = transform.root.gameObject.GetComponent<PlayerController>();
        playerAnim = transform.root.gameObject.GetComponent<Animator>();
        shieldBehavior = shield.GetComponent<Shield>();
        shieldAnim = shield.GetComponent<Animator>();

        shieldBehavior.shield.SetActive(false);
    }

    private void Update() {
        if(Input.GetButton("Fire2") && pphysics.IsGrounded())
            if(shieldBehavior.shieldCurrentHealth > shieldBehavior.shieldMaxHealth / 2 || blocking)
                Shield();
        if(Input.GetButtonUp("Fire2"))
            DisableShield();
    }

    private void Shield() {
        playerAnim.SetBool("blocking", true);
        shieldAnim.SetBool("blocking", true);

        shieldBehavior.shield.SetActive(true);
        col.enabled = true;
        blocking = true;
        
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        transform.root.LookAt(transform.root.position + forward);
        //player.canMove = false;
    }

    public void DisableShield() {
        playerAnim.SetBool("blocking", false);
        shieldAnim.SetBool("blocking", false);
        col.enabled = false;
        blocking = false;
        //player.canMove = true;
    }
}

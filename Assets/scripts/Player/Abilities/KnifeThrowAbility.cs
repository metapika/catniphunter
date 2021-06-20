using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrowAbility : MonoBehaviour
{
    #region Fields
    public GameObject knifeObj;
    private LineRenderer line;
    private PlayerCombat combat;
    private Animator anim;
    public float range;
    public float timeBetweenAttacks = 1f;
    public GameObject knifePrefab;
    private bool canShoot = true;
    private Vector3 cameraTargetDef = new Vector3(0f, 0.2f, 0f);
    private Vector3 cameraTargetAim = new Vector3(0.5f, 0.6f, 0.3f);
    private Vector3 cameraDef = new Vector3(0f, 0f, -2.5f);
    private Vector3 cameraAim = new Vector3(0f, 0f, -0.8f);
    private bool rightShoulder = true;
    [HideInInspector] public bool aiming;
    [HideInInspector] public Transform target;
    [HideInInspector] public Transform knifeThrowPoint;

    #endregion

    #region Unity Functions
    private void Awake() {
        combat = transform.root.GetComponent<PlayerCombat>();
        anim = transform.root.GetComponent<Animator>();
        line = GetComponent<LineRenderer>();

        //knifeThrowPoint = combat.knifeThrowPoint;
    }
    private void Update() {
        KnifeAbility();

        if(aiming) {
            transform.parent.parent.GetComponent<PlayerController>().canRotate = false;
            if(transform.parent.parent.GetComponent<PlayerCombat>().currentWeapon != null && transform.root.GetComponent<PlayerCombat>().currentWeapon.GetComponent<Melee>() != null) {
                // transform.parent.parent.GetComponent<PlayerCombat>().currentWeapon.GetComponent<Melee>().canAttack = false;
                //transform.parent.parent.GetComponent<PlayerCombat>().currentWeapon.GetComponent<Melee>().UnequipSword();
            }
        } else {
            transform.parent.parent.GetComponent<PlayerController>().canRotate = true;
            // if(transform.parent.parent.GetComponent<PlayerCombat>().currentWeapon != null && transform.root.GetComponent<PlayerCombat>().currentWeapon.GetComponent<Melee>() != null) {
            //     transform.parent.parent.GetComponent<PlayerCombat>().currentWeapon.GetComponent<Melee>().canAttack = true;
            // }
        }
    }
    private void LateUpdate() {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0));
       
        if(aiming) {
            anim.GetBoneTransform(HumanBodyBones.Spine).LookAt(rayOrigin + (Camera.main.transform.forward * range));
            anim.GetBoneTransform(HumanBodyBones.Chest).LookAt(rayOrigin + (Camera.main.transform.forward * range));
            anim.GetBoneTransform(HumanBodyBones.UpperChest).LookAt(rayOrigin + (Camera.main.transform.forward * range));
            anim.GetBoneTransform(HumanBodyBones.Neck).LookAt(rayOrigin + (Camera.main.transform.forward * range));
            anim.GetBoneTransform(HumanBodyBones.Head).LookAt(rayOrigin + (Camera.main.transform.forward * range));
        }

        if(knifeObj != null) {
            line.SetPosition(0, knifeObj.transform.Find("gfx").position);
            line.SetPosition(1, rayOrigin + (Camera.main.transform.forward * range));
        }
    }

    #endregion
    
    #region Custom Functions
    private void KnifeAbility() {
        if(Input.GetButtonDown("Aim")) {
            combat.crossair.SetActive(true);
        }

        if(Input.GetButton("Aim")) {
            
            anim.SetBool("aiming", true);
            aiming = true;

            Camera.main.transform.localPosition = cameraAim;
            if(rightShoulder) {
                target.localPosition = cameraTargetAim;
            } else {
                target.localPosition = new Vector3(-cameraTargetAim.x, cameraTargetAim.y, cameraTargetAim.z);
            }

            if(canShoot) {
                line.enabled = true;
                if(knifeObj == null) {
                    knifeObj = Instantiate(knifePrefab, combat.handR.position, new Quaternion(0, 0, 0, 0), combat.handR);
                }
            } else {
                line.enabled = false;
            }
            if(knifeObj != null) {
                knifeObj.SetActive(true);
                knifeObj.GetComponent<ThrowingKnife>().trail.GetComponent<TrailRenderer>().enabled = false;
            }
            
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            transform.root.LookAt(transform.position + forward);

            if(Input.GetButton("Throw")) {
                if(canShoot) {
                    StartCoroutine(ThrowKnife());
                }
            }
        }

        if(Input.GetButtonUp("Aim")) {
            anim.SetBool("aiming", false);
            if(knifeObj != null) {
                knifeObj.SetActive(false);
                knifeObj.GetComponent<ThrowingKnife>().trail.GetComponent<TrailRenderer>().enabled = false;
            }
            line.enabled = false;

            aiming = false;

            Camera.main.transform.localPosition = cameraDef;
            target.localPosition = cameraTargetDef;

            combat.crossair.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.V)) {
            if(rightShoulder) {
                rightShoulder = false;
            } else {
                rightShoulder = true;
            }
        }
    }

    private IEnumerator ThrowKnife() {
        canShoot = false;
        line.enabled = false;
        anim.SetTrigger("throw");

        if(knifeObj != null) {
            knifeObj.transform.parent = null;
            knifeObj.GetComponent<ThrowingKnife>().trail.GetComponent<TrailRenderer>().enabled = true;
            knifeObj.transform.rotation = knifeThrowPoint.rotation;
            knifeObj.GetComponent<ThrowingKnife>().enabled = true;

            knifeObj = null;
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        canShoot = true;
        line.enabled = true;
    }

    #endregion
}

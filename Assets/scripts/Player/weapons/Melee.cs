using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Melee : MonoBehaviour
{
    #region Fields
    public Weapon_SO weaponDefinition;
    public float[] attacksKnockBackStrenght = new float[3];
    public bool canAttack;
    public float timeStopDuration = 0.1f;
    public float minKnifeKillDistance = 0.3f;
    public ParticleSystem trail;
    public GameObject hitParticles;
    private PlayerCombat combat;
    private Animator anim;
    
    [Header("Only check this if the weapon is double swords")]    
    public Transform sword1;
    public Transform sword2;
    private PlayerPhysics pphysics;

    #endregion

    #region Unity Functions
    private void Awake() {
        if(transform.parent.CompareTag("WeaponHolder")) {
            InitializeEquip();
            UnequipSword();
        }

        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) 
        {
            sword1.GetComponent<DoubleSword>().weaponDefinition = weaponDefinition;
            sword2.GetComponent<DoubleSword>().weaponDefinition = weaponDefinition;
        }
    }
    private void Update() {
        if(Time.timeScale <= 0) return;
        
        if(canAttack && transform.parent != null) {
            if(Input.GetButtonDown("Attack") && transform.parent.CompareTag("WeaponHolder")) {
                // && pphysics.IsGrounded()
                pphysics.movement.UnCrouch();
                EquipSword();

                Attack();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Enemy")) {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if(!enemy.dead) {
                enemy.TakeDamage(weaponDefinition.damage, transform.root);
                Instantiate(hitParticles, other.ClosestPoint(transform.position), transform.rotation);
                combat.Stop(timeStopDuration);

                // if(!other.GetComponent<SpiderStats>().gettingKnockbacked && !other.GetComponent<SpiderStats>().dead) {
                //     other.GetComponent<SpiderStats>().TakeDamage(weaponDefinition.damage, transform.root);
                //     Instantiate(hitParticles, other.ClosestPoint(transform.position), transform.rotation);
                //     StartCoroutine(other.GetComponent<SpiderStats>().Knockback(transform, attacksKnockBackStrenght[attacknum -1]));
                // }
            }
        }
        if(other.CompareTag("VentOpening")) {
            Destroy(other.gameObject);
        }
    }

    #endregion

    #region Custom Functions
    int attacknum = 1;
    private void Attack() {
        attacknum++;
        if(attacknum == 4)
        {
            attacknum = 1;
        }

        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.Knife) {
            if(combat.targets.Count > 0) {
                if(combat.camControl.CameraToggleState() && Vector3.Distance(transform.root.position, combat.lockOnTarget.position) > minKnifeKillDistance) {
                    return;
                } 
                else if(!combat.camControl.CameraToggleState() && Vector3.Distance(transform.root.position, combat.nearestTarget.position) > minKnifeKillDistance) {
                    return;
                }
            }
        }
        
        if(canAttack) {
            if(weaponDefinition.multipleAnimations) {
                anim.SetTrigger(weaponDefinition.animationTrigger + attacknum.ToString());
            } else {
                anim.SetTrigger(weaponDefinition.animationTrigger);
            }
        }
    }
    public void InitializeEquip() {
        combat = transform.root.GetComponent<PlayerCombat>();
        pphysics = transform.root.GetComponent<PlayerPhysics>();
        anim = transform.root.GetComponent<Animator>();
    }

    public void UnequipSword() {
        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            transform.SetParent(combat.weaponCase);
            transform.position = combat.weaponCase.position;
            transform.rotation = combat.weaponCase.rotation;
        }

        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            sword1.SetParent(transform);
            sword2.SetParent(transform);

            sword1.localPosition = new Vector3(0.4233348f, 0.6556629f, 0.09347475f);
            sword1.localEulerAngles = new Vector3(108.693f, 89.485f, -49.12601f);

            sword2.localPosition = new Vector3(-0.5248085f, 0.5959191f, 0.004204407f);
            sword2.localEulerAngles = new Vector3(61.839f, 107.734f, -209.335f);
        }
    }
    
    public void EquipSword() {
        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            transform.SetParent(combat.handR);
            transform.position = combat.handR.position;
            transform.rotation = combat.handR.rotation;
        }

        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            sword1.SetParent(combat.handR);
            sword2.SetParent(combat.handL);

            sword1.localPosition = new Vector3(0.008f, -0.008f, 0.008f);
            sword1.localEulerAngles = new Vector3(-48.504f, -23.417f, 52.46f);

            sword2.localPosition = new Vector3(-0.013f, -0.013f, -0.008f);
            sword2.localEulerAngles = new Vector3(7.161f, -24.921f, 40.121f);
        }
    }
    #endregion

}

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
    public ParticleSystem trail;
    public GameObject hitParticles;
    private PlayerCombat combat;
    private Animator anim;
    
    [Header("Only check this if the weapon is double swords")]    
    public Transform sword1;
    public Transform sword2;

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
        if(canAttack && transform.parent != null) {
            if(Input.GetButtonDown("Attack") && transform.parent.CompareTag("WeaponHolder")) {
                EquipSword();

                Attack();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Enemy")) {
            other.GetComponent<SpiderStats>().TakeDamage(weaponDefinition.damage);
            Instantiate(hitParticles, other.ClosestPoint(transform.position), transform.rotation);
            
            if(!other.GetComponent<SpiderStats>().gettingKnockbacked) {
                StartCoroutine(other.GetComponent<SpiderStats>().Knockback(transform, attacksKnockBackStrenght[attacknum -1]));
                
                Rigidbody rb = other.GetComponent<Rigidbody>();

                if(rb != null)
                {
                    Vector3 direction = other.transform.position - transform.root.position;
                    direction.y = 0;

                    rb.AddForce(direction.normalized * attacksKnockBackStrenght[attacknum -1], ForceMode.Impulse);
                }
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
            transform.SetParent(combat.weaponHand);
            transform.position = combat.weaponHand.position;
            transform.rotation = combat.weaponHand.rotation;
        }

        if(weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            sword1.SetParent(combat.weaponHand);
            sword2.SetParent(combat.weaponHand2);

            sword1.localPosition = new Vector3(0.008f, -0.008f, 0.008f);
            sword1.localEulerAngles = new Vector3(-48.504f, -23.417f, 52.46f);

            sword2.localPosition = new Vector3(-0.013f, -0.013f, -0.008f);
            sword2.localEulerAngles = new Vector3(7.161f, -24.921f, 40.121f);
        }
    }
    #endregion

}

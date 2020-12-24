using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    [Header("Sentry Type")]
    public bool cannon = true;
    public bool missle;

    [Space]

    [Header("Projectiles")]
    public GameObject cannonProjectile;
    public GameObject missleProjectile;

    [Space]

    [Header("Variables")]
    public bool activated;
    public bool attack;
    public float rotationSpeed;
    public Transform rifles;
    public float timeBetweenAttacksCannon = 0.2f;
    public float timeBetweenAttacksMissle = 2f;

    public List<Transform> gunPoints;
    public float sightRange;
    public LayerMask whatIsPlayer;

    [Space]

    private Animator anim;
    private bool alreadyAttacked;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        DetectPlayer();

        if(attack)
            AttackMode();

        HandleAnimations();
    }

    public void DetectPlayer() {
        if(Physics.CheckSphere(transform.position, sightRange, whatIsPlayer)) {
            activated = true;
        } else {
            activated = false;
        }
    }

    public void AttackMode() {
        if(cannon)
            rifles.Rotate(0, rotationSpeed, 0);

        if(!alreadyAttacked) {
            if(cannon)
            {
                foreach (Transform rifle in gunPoints)
                {
                    var horizontalRotationOffset = Quaternion.Euler(0, Random.Range(-25, 25), 0);
                    BulletBase bullet = Instantiate(cannonProjectile, rifle.position, rifle.rotation * horizontalRotationOffset).GetComponent<BulletBase>();
                    bullet.gunPoint = rifle;

                } 
                alreadyAttacked = true;
                StartCoroutine(ResetAttack(timeBetweenAttacksCannon));
            }

            else if(missle)
            {
                foreach (Transform rifle in gunPoints)
                {
                    // Quaternion rotation = new Quaternion(rifle.rotation.x, rifle.rotation.y + 90, rifle.rotation.z, rifle.rotation.w);
                    BulletBase bullet = Instantiate(missleProjectile, rifle.position, rifle.rotation).GetComponent<BulletBase>(); 
                }
                alreadyAttacked = true;
                StartCoroutine(ResetAttack(timeBetweenAttacksMissle));
            }

        }
    }

    private IEnumerator ResetAttack(float offset)
    {
        yield return new WaitForSeconds(offset);

        alreadyAttacked = false;
    }

    private void HandleAnimations() {
        if(activated)
            anim.SetBool("activated", true);
        else if(!activated)
            anim.SetBool("activated", false);
        
        if(attack)
            anim.SetBool("attack", true);
        else if(!attack)
            anim.SetBool("attack", false);
    }

    public void EnableAttacking() {
        attack = true;
    }

    public void DisableAttacking() {
        attack = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Missile : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float force;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float rotationForce;
    [SerializeField] private float timeBeforeHoming = 1f;
    public int bulletDamage = 30;

    [SerializeField] private float shakeForce;
    [SerializeField] private float shakeDuration;
    [SerializeField] private int shakeRandomness;

    [SerializeField] private ParticleSystem explosionParticle1 = null;
    [SerializeField] private ParticleSystem explosionParticle2 = null;
    private bool shouldFollow;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Camera cam;

    private float randomRotation;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        target = GameObject.Find("RoboSamurai").transform;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        StartCoroutine(WaitBeforeHoming());

        randomRotation = Random.Range(rotationForce / 2, rotationForce);
    }

    private void FixedUpdate() 
    {
        if(shouldFollow) {
            if(target != null)
            {
                Vector3 direction = target.position - rb.position;
                direction.Normalize();
                Vector3 rotationAmount = Vector3.Cross(transform.forward, direction);
                rb.angularVelocity = rotationAmount * randomRotation;
                rb.velocity = transform.forward * force;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("Collided with" + other.gameObject.name);
        if(other.gameObject != target)
            DestroyMissile();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Melee"))
            DestroyMissile();
    }

    public void DestroyMissile() {
        //Play explosion effect
        cam.DOShakePosition(shakeDuration, shakeForce, shakeRandomness);
        Instantiate(explosionParticle1, transform.position, Quaternion.identity);
        Instantiate(explosionParticle2, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator WaitBeforeHoming() {
        rb.AddForce(transform.forward * launchSpeed, ForceMode.Impulse);
        col.enabled = false;
        yield return new WaitForSeconds(timeBeforeHoming);
        col.enabled = true;
        shouldFollow = true;
    }
}

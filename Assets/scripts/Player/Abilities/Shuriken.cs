using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed;
    [HideInInspector] public float shurikenForce;
    private Rigidbody rb;
    public Transform model;
    public Transform target;
    public float shurikenRemoveTime = 10f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DestroyShuriken());
    }

    private void Update() {
        model.Rotate(0, rotationSpeed, 0);
    }
    void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 direction = target.position - transform.position;
            rb.AddForce(direction * shurikenForce * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.forward * shurikenForce * Time.deltaTime, ForceMode.Impulse);
        }
    }

    public IEnumerator DestroyShuriken() {
        yield return new WaitForSeconds(shurikenRemoveTime);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 8)
            Destroy(gameObject);
    }
}

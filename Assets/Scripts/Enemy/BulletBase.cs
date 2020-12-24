using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float bulletSpeed = 50f;
    public float bulletRemoveTime = 4f; 
    public int bulletDamage;
    public Transform gunPoint;
    public Rigidbody rb;
    
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    public IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(bulletRemoveTime);

        Destroy(gameObject);
    }
}

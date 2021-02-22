using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float bulletSpeed = 50f;
    public float bulletRemoveTime = 4f; 
    public int bulletDamage;
    public Rigidbody rb;

    public Vector3 prevPos;
    
    void Start()
    {
        StartCoroutine(DestroyBullet());

        prevPos = transform.position;
    }

    private void Update() {
        prevPos = transform.position;

        transform.Translate(0.0f, 0.0f, bulletSpeed * Time.deltaTime);

        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);
        
        for (int i = 0; i < hits.Length; i++)
        {
            PlayerStats health = hits[i].collider.GetComponent<PlayerStats>();
            Shield shield = hits[i].collider.GetComponent<Shield>();


            if(!hits[i].collider.gameObject.CompareTag("Player") && !hits[i].collider.gameObject.CompareTag("Shield")) {
                Destroy(gameObject);
                return;
            }

            if(shield != null) {
                shield.BlockDamage(bulletDamage * 3);
                Destroy(gameObject);
                return;
            }

            if(health != null) {
                health.TakeDamage(bulletDamage);
                Destroy(gameObject);
            }

        }
    }

    // void FixedUpdate()
    // {
    //     rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    // }

    public IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(bulletRemoveTime);

        Destroy(gameObject);
    }
}

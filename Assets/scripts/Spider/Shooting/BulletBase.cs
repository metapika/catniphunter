using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase: MonoBehaviour
{
    public float bulletSpeed = 50f;
    public float bulletRemoveTime = 4f; 
    public int bulletDamage;
    public GameObject hitParticles;
    public Transform enemy;

    public Vector3 prevPos;
    
    void Start()
    {
        StartCoroutine(DestroyBullet());

        prevPos = transform.position;
    }

    private void Update() {
        prevPos = transform.position;

        transform.Translate(0f, 0f, bulletSpeed * Time.deltaTime);
        
        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);
        
        for (int i = 0; i < hits.Length; i++)
        {

            if(!hits[i].collider.gameObject.CompareTag("Player") && !hits[i].collider.gameObject.CompareTag("Enemy")) {
                if(hits[i].collider.gameObject.CompareTag("Shield"))
                {
                    hits[i].transform.GetComponent<Shield>().BlockDamage(bulletDamage);
                    Destroy(gameObject);
                }
            } else if(hits[i].collider.gameObject.CompareTag("Player")) 
            {
                hits[i].transform.GetComponent<PlayerStats>().ParryDecision(bulletDamage, enemy);
                Destroy(gameObject);
                return;
            }
        }
        //Instantiate(hitParticles, transform.position, Quaternion.identity);
    }

    public IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(bulletRemoveTime);

        Destroy(gameObject);
    }
}

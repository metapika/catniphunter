using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase: MonoBehaviour
{
    public float bulletSpeed = 50f;
    public float bulletRemoveTime = 4f; 
    public int bulletDamage;
    public string bulletTag = "BlasterBullet";
    public GameObject hitParticles;
    [HideInInspector] public Transform enemy;

    public Vector3 prevPos;
    private ObjectPooler objectPooler;
    
    private void Start() {
        objectPooler = ObjectPooler.instance;
    }
    void OnEnable()
    {
        StartCoroutine(DestroyBullet());

        prevPos = transform.position;
    }

    private void Update() {
        prevPos = transform.position;

        transform.Translate(0f, 0f, bulletSpeed * Time.deltaTime);
        
        RaycastHit hit;
        
        
        if(Physics.Raycast(new Ray(prevPos, (transform.position - prevPos).normalized), out hit, (transform.position - prevPos).magnitude))
        {
            if(hit.collider.gameObject.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerStats>().ParryDecision(bulletDamage, enemy);
                gameObject.SetActive(false);
                return;
            }
            else if(hit.collider.gameObject.CompareTag("Shield"))
            {
                hit.transform.GetComponent<Shield>().BlockDamage(bulletDamage);
                gameObject.SetActive(false);
                return;
            }
            else if(hit.collider.gameObject.CompareTag("Enemy") || hit.collider.gameObject.CompareTag("Bullet"))
            {
                return;
            } else {
                gameObject.SetActive(false);
            }
        }

        //Instantiate(hitParticles, transform.position, Quaternion.identity);
    }

    public IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(bulletRemoveTime);

        gameObject.SetActive(false);
    }
}

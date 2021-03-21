using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    public float knifeSpeed = 17f;
    public float knifeRemoveTime = 4f; 
    public float knifeGfxRotateSpeed = 20f;
    public int knifeDamage;
    public Transform trail;
    public Transform gfx;
    private Vector3 prevPos;

    private void Awake() {
        StartCoroutine(DestroyBullet());

        prevPos = transform.position;
    }
    private void Update() {
        prevPos = transform.position;

        transform.Translate(transform.forward * knifeSpeed * Time.deltaTime, Space.World);
        
        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);
        
        for (int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform.CompareTag("Bouncable"))
            {
                var newDir = Vector3.Reflect(this.transform.forward, hits[i].normal);
                newDir = Vector3.ProjectOnPlane(newDir, Vector3.up);
                
                float rotationz = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationz - 90);

                this.transform.forward = newDir;
            }

            if(hits[i].transform.CompareTag("Enemy")) {
                hits[i].transform.GetComponent<SpiderStats>().TakeDamage(knifeDamage);
                Destroy(gameObject);
            }

            if(hits[i].transform.CompareTag("PressurePad")) {
                hits[i].transform.GetComponent<PressurePad>().Activate();
                Destroy(gameObject);
            }
        }
    }
    public IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(knifeRemoveTime);

        Destroy(gameObject);
    }
}

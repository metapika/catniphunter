using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float knockbackStrenght = 5f;

    private void OnTriggerEnter(Collider other) {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if(other.gameObject.CompareTag("Enemy"))
            Debug.Log("psie");

        if(rb != null)
        {
            Vector3 direction = other.transform.position - transform.root.position;

            rb.AddForce(direction.normalized * knockbackStrenght, ForceMode.Impulse);
        }
    }
}

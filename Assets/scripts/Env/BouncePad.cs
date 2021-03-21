using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce;
    [SerializeField] private Vector3 direction;

    // Update is called once per frame
    void Update() {
        direction = transform.up;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            
            PlayerPhysics pphysics = other.gameObject.GetComponent<PlayerPhysics>();
            pphysics.velocity = direction * Mathf.Sqrt(bounceForce * -2f * pphysics.currentGravity);
        }
    }
}

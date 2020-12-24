using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float currentSpeed;

    public Transform model;

    public float rotationSpeedY = 1f;
    public float movementSpeedY = 1f;
    
    private float origY;
    public float maxDistance = 0.6f;
    
    private Rigidbody rb;

    void Start() {   
        origY = transform.position.y;
        currentSpeed = -movementSpeedY;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (origY - transform.position.y > maxDistance)
        {
            currentSpeed = movementSpeedY;
        }
        else if (origY - transform.position.y < -maxDistance)
        {
            currentSpeed = -movementSpeedY;
        }

        model.Rotate(0f, rotationSpeedY, 0f);
        transform.Translate(0, currentSpeed * Time.deltaTime, 0);
    }
}

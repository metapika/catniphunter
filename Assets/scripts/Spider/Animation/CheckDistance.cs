using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistance : MonoBehaviour
{
    public Transform target;
    public Transform IkTarget;
    public Color lineColor = Color.red;
    public bool debug;
    public float calculatedDistance;

    void Update()
    {
        if(debug) {
            Debug.DrawLine(transform.position, target.position, lineColor);
        }

        calculatedDistance = Vector3.Distance(IkTarget.position, target.position);
    }

    public float Distance() {
        return calculatedDistance;
    }
}

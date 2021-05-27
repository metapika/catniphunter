using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMovementTarget : MonoBehaviour
{
    public LayerMask env;

    void Update()
    {
        RaycastHit hit;
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        if(Physics.Raycast(startPos, -Vector3.up, out hit, 100, env)) 
        {
            transform.position = hit.point;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBodyUp : MonoBehaviour
{
    public LayerMask env;
    public Transform body;

    void Update()
    {
        RaycastHit hit;
        Vector3 positiveOffset = new Vector3(0f, 5f, 0f);

        if(Physics.Raycast(transform.position + positiveOffset, -Vector3.up, out hit, 1000f, env))
        {
            //float distanceFromGround = Vector3.Distance(transform.position, hit.point);
            Vector3 offset = new Vector3(0f, 0.3f, 0f);

            body.position = hit.point + offset;
        }
    }
}

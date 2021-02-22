using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMovement : MonoBehaviour
{
    public CheckDistance legDistanceReport;
    public float stepDistance = 4;
    public float smoothTime = 0.3F;
    private float legDistanceCalculated;
    private Vector3 velocity = Vector3.zero;
    Vector3 footTarget = Vector3.zero;

    void Update()
    {
        float legDistanceCalculated = legDistanceReport.Distance();

        if (legDistanceCalculated > stepDistance) {
            footTarget = legDistanceReport.target.position;
        }

        if (
        // has completed last step.
        Vector3.Distance(footTarget, legDistanceReport.IkTarget.position) < 0.01f &&
        // is ready to do next step.
        legDistanceCalculated > stepDistance) 
        {
            footTarget = legDistanceReport.IkTarget.position;
        }

        legDistanceReport.IkTarget.position = Vector3.SmoothDamp(legDistanceReport.IkTarget.position, footTarget, ref velocity, smoothTime );
        // legDistanceReport.IkTarget.position = Vector3.SmoothDamp(legDistanceReport.IkTarget.position, footTarget, ref velocity, smoothTime);     
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public List<Transform> patrolPoints = new List<Transform>();
    private void Start() 
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            patrolPoints.Add(transform.GetChild(i));
        }
    }
}

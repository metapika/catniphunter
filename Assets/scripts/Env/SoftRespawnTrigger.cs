using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftRespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) 
        {
            other.GetComponent<PlayerStats>().SoftRespawn();
        }
    }
}

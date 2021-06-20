using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftRespawnTrigger : MonoBehaviour
{
    [SerializeField] private CheckpointManagement checkpointManagement;
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) 
        {
            checkpointManagement.SoftRespawn();
        }
    }
}

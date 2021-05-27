using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    public Door connectedDoor;
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player"))
        {
            //Play some animation
            connectedDoor.KeyCollected();
            Destroy(gameObject);
        }
    }
}

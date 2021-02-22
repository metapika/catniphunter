using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            other.transform.Find("Death").GetComponent<Die>().CommitDie("waterDeath");
        }
    }
}

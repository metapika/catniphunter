using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public Material activatedMaterial;
    public MeshRenderer pressurePart;
    public bool activated;
    public Vector3 pressedPosition;
    public Door connectedDoor;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Player")) {
            Activate();
        }
    }

    public void Activate() {
        if(!activated) {
            activated = true;
            pressurePart.material = activatedMaterial;
            pressurePart.transform.localPosition = pressedPosition;
            if(connectedDoor != null) 
            {
                connectedDoor.padsActivated++;

                if(connectedDoor.padsActivated >= connectedDoor.connectedPressurePads.Count) {
                    Destroy(connectedDoor.doorGfx);
                }
            }
        }
    }
}

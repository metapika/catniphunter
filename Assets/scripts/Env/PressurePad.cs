using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public Material activatedMaterial;
    public MeshRenderer pressurePart;
    public bool activated;
    private bool puzzleComplete;
    [HideInInspector] public Door connectedDoor;
    private Material defMaterial;
    private void Start() {
        defMaterial = pressurePart.material;
    }
    private void OnTriggerEnter(Collider other) {
        if(puzzleComplete) return;

        if(other.CompareTag("Player") || other.CompareTag("Crate")) 
        {
            Activate();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(puzzleComplete) return;

        if(other.CompareTag("Player") || other.CompareTag("Crate")) 
        {
            Disactivate();
        }
    }

    public void Activate() {
        activated = true;
        pressurePart.material = activatedMaterial;
        pressurePart.transform.localPosition = new Vector3(0f, 0.26f, 0f);
        if(connectedDoor != null) 
        {
            int activatedPads = 0;
            foreach(PressurePad pad in connectedDoor.connectedPressurePads) {
                if(pad.activated) {
                    activatedPads++;
                }
            }
            if(activatedPads >= connectedDoor.connectedPressurePads.Count) {
                connectedDoor.doorType = DoorType.Auto;
                puzzleComplete = true;
            }
        }
    }
    public void Disactivate()
    {
        activated = false;
        pressurePart.material = defMaterial;
        pressurePart.transform.localPosition = new Vector3(0f, 0.8382198f, 0f);
    }
}

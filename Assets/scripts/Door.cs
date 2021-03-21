using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<PressurePad> connectedPressurePads = new List<PressurePad>();
    public GameObject doorGfx;
    public int padsActivated;
    
    private void Start() {
        foreach(PressurePad pad in connectedPressurePads) {
            pad.connectedDoor = this;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
    Auto,
    Key,
    PressurePad
}
public class Door : MonoBehaviour
{
    public DoorType doorType = DoorType.Auto;
    public bool opened;
    public GameObject doorGfx;
    private Animator anim;

    [Header("Check this if the door is a pressure pad type door")]
    public List<PressurePad> connectedPressurePads = new List<PressurePad>();
    
    private void Start() {
        anim = GetComponent<Animator>();

        if(doorType != DoorType.PressurePad) return;
        
        foreach(PressurePad pad in connectedPressurePads) {
            pad.connectedDoor = this;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(doorType != DoorType.Auto) return;

        if(other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(doorType != DoorType.Auto) return;

        if(other.CompareTag("Player"))
        {
            CloseDoor();
        }
    }
    public void OpenDoor()
    {
        opened = true;
        anim.SetBool("opened", true);
    }

    public void CloseDoor()
    {
        opened = false;
        anim.SetBool("opened", false);
    }
    public void KeyCollected()
    {
        doorType = DoorType.Auto;
    }
}

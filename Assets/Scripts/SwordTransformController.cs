using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTransformController : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private CharacterController playerCC;
    [SerializeField] private BoxCollider Cutter;
    private void Start() {
        anim = GetComponent<Animator>();
        Cutter.enabled = false;
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Swing");
        }
    }
    public void ResetTrigger()
    {
        anim.ResetTrigger("Swing");
    }
    public void EnableCutter()
    {
        Cutter.enabled = true;
    }
    public void DisableCutter()
    {
        Cutter.enabled = false;
    }
}

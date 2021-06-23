using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DodgeAbility : MonoBehaviour
{
    public bool useParticles = false;
    public float dodgeTime = 0.35f;
    public float dodgeSpeed = 15f; 
    public int dodgeCount = 2;
    public int currentDodgeCount = 0;
    public float woreOffTimer = 0;
    public float woreOffDestination = 1f;
    public ParticleSystem dodgeParticles;
    public float distortionAmount = 0.4f;

    private GameObject PostProcess;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private PlayerStats stats;
    private PlayerCombat combat;
    
    void Start() {
        movement = transform.root.gameObject.GetComponent<PlayerController>();
        pphysics = transform.root.gameObject.GetComponent<PlayerPhysics>();
        stats = transform.root.gameObject.GetComponent<PlayerStats>();
        combat = transform.root.gameObject.GetComponent<PlayerCombat>();

        Camera m_MainCamera = Camera.main;

        dodgeParticles.gameObject.SetActive(false);
        dodgeParticles.transform.SetParent(m_MainCamera.transform);
        dodgeParticles.transform.position = m_MainCamera.transform.position;
        dodgeParticles.transform.rotation = m_MainCamera.transform.rotation;

        PostProcess = GameObject.Find("PostProcess");
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.LeftShift) && pphysics.IsGrounded() && stats.GetCurrentSpeed() == stats.GetSprintSpeed()) {
            if(currentDodgeCount < dodgeCount) {
                combat.DodgeInput();
                StartCoroutine(Dodge());
            }
        }

        //Cooldown dodge
        if(currentDodgeCount > 0) {
            woreOffTimer += Time.deltaTime;
        }
        
        if(woreOffTimer > woreOffDestination && currentDodgeCount == dodgeCount || woreOffTimer > woreOffDestination / 2 && currentDodgeCount < dodgeCount) {
            woreOffTimer = 0;
            currentDodgeCount = 0;
        }
    }

    private IEnumerator Dodge() {
        float startTime = Time.time;
        currentDodgeCount++;
        
        Volume volume = null;
        LensDistortion Distortion = null;

        if(PostProcess != null) {
            volume = PostProcess.GetComponent<Volume>();
        }

        if (volume != null && volume.profile.TryGet<LensDistortion>(out var disVar)) {
            Distortion = disVar;
            Distortion.intensity.overrideState = true;
            Distortion.intensity.value = distortionAmount;
        }

        //Invincibility
        //transform.root.gameObject.GetComponent<PlayerStats>().enabled = false;

        //Particles
        if(useParticles) {
            dodgeParticles.gameObject.SetActive(true);
        }

        while(Time.time < startTime + dodgeTime)
        {
            movement.controller.Move(movement.moveDir * dodgeSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        transform.root.gameObject.GetComponent<PlayerStats>().enabled = true;
        dodgeParticles.gameObject.SetActive(false);

        if (Distortion != null){
            Distortion.intensity.overrideState = true;
            Distortion.intensity.value = 0f;
        }
    }
}

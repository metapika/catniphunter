using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirDashAbility : MonoBehaviour, IHasCooldown
{
    private CooldownManager cooldownSystem;
    [Header("AirDash Cooldown Variables")]
    public int abilityId = 2;
    public float dashCooldownDuration = 2f;
    public int Id => abilityId;
    public float CooldownDuration => dashCooldownDuration;

    [Space]
    public Slider dashSlider;
    public Text dashCountdownText;
    private Animator playerAnim;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private PlayerStats stats;

    [Space]

    [Header("Mid-air dash Variables")]
    public float mass = 3f;
    Vector3 impact = Vector3.zero;
    [SerializeField] private float airDashSpeed = 15f;

    private void Awake() {
        playerAnim = transform.root.gameObject.GetComponent<Animator>();
        movement = transform.root.gameObject.GetComponent<PlayerController>();
        pphysics = transform.root.gameObject.GetComponent<PlayerPhysics>();
        stats = transform.root.gameObject.GetComponent<PlayerStats>();
        cooldownSystem = transform.parent.GetComponent<CooldownManager>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !pphysics.IsGrounded()) {
            if(!cooldownSystem.IsOnCooldown(abilityId)) {
                cooldownSystem.PutOnCooldown(this);
                playerAnim.SetBool("dashing", true);
                StartCoroutine(AirDash());
            }
        }
        
        if (impact.magnitude > 0.2) movement.controller.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
        if(dashSlider != null) {
            if(cooldownSystem.GetRemainingDuration(abilityId) != 0)
                UpdateCooldownIndicator(abilityId, dashSlider, dashCountdownText);
            else
                dashCountdownText.gameObject.SetActive(false);
        }
    }

    public void AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
            impact += dir.normalized * force / mass;
    }
    
    private void UpdateCooldownIndicator(int id, Slider cooldownSlider, Text countdownText) {
        dashCountdownText.gameObject.SetActive(true);
        cooldownSlider.value = -(cooldownSystem.GetRemainingDuration(id));
        countdownText.text = Mathf.RoundToInt(cooldownSystem.GetRemainingDuration(id)).ToString();
    }

    private IEnumerator AirDash() {
        AddImpact(movement.moveDir, airDashSpeed);
        playerAnim.SetBool("dashing", false);
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShurikenAbility : MonoBehaviour, IHasCooldown
{
    private CooldownManager cooldownSystem;
    [Header("Shuriken Cooldown and Spawning Variables")]
    [SerializeField] private Transform shurikenSpawnPoint = null;
    [SerializeField] private GameObject shuriken = null;
    public int abilityId = 1;
    public float shurikenCooldownDuration = 5f;
    public float timeBetweenThrowing = 0.2f;
    public int Id => abilityId;
    public float CooldownDuration => shurikenCooldownDuration;
    private PlayerCombat combat;

    [Space]
    
    [Header("Shuriken Properties")]
    public float rotationSpeed = 10f;
    public float shurikenForce = 10f;
    public Slider shurikenSlider;
    public Text shurikenCountdownText;
    private Animator playerAnim;

    private void Awake() {
        playerAnim = transform.root.gameObject.GetComponent<Animator>();
        combat = transform.root.gameObject.GetComponent<PlayerCombat>();

        cooldownSystem = transform.parent.GetComponent<CooldownManager>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q))
            if(!cooldownSystem.IsOnCooldown(abilityId))
                ThrowShurikens();
        if(shurikenSlider != null) {
            if(cooldownSystem.GetRemainingDuration(abilityId) != 0)
                UpdateCooldownIndicator(abilityId, shurikenSlider, shurikenCountdownText);
            else
                shurikenCountdownText.gameObject.SetActive(false);
        }
    }

    private void ThrowShurikens()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        transform.LookAt(transform.position + forward);
        playerAnim.SetTrigger("ability1");

        GameObject projectileInstance = Instantiate(shuriken, shurikenSpawnPoint.position, shurikenSpawnPoint.rotation);
        projectileInstance.GetComponent<Shuriken>().rotationSpeed = rotationSpeed;
        projectileInstance.GetComponent<Shuriken>().shurikenForce = shurikenForce;

        if(combat.targets.Count > 0)
        {
            projectileInstance.GetComponent<Shuriken>().target = combat.nearestTarget.transform;
        } else
        {
            projectileInstance.GetComponent<Shuriken>().target = null;
        }
            

        cooldownSystem.PutOnCooldown(this);
    }
    private void UpdateCooldownIndicator(int id, Slider cooldownSlider, Text countdownText) {
        shurikenCountdownText.gameObject.SetActive(true);
        cooldownSlider.value = -(cooldownSystem.GetRemainingDuration(id));
        countdownText.text = Mathf.RoundToInt(cooldownSystem.GetRemainingDuration(id)).ToString();
    }
}

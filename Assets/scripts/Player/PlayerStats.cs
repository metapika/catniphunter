using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Scene;
using UnityCore.Menu;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    #region Fields
    [Header("Health and speed values")]
    public float currentHealth = 0;
    [SerializeField] private int maxHealth = 0;
    [SerializeField] private float autoRegenAmount = 0.25f;
    [SerializeField] private bool canAutoRegen = true;
    [SerializeField] private float outOfCombatResetTime = 10;
    [SerializeField] private float outOfCombatTimer = 0;
    [SerializeField] private float outOfCombatRate = 0.25f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float crouchSpeed = 30f;
    [SerializeField] private float walkSpeed = 60f;
    [SerializeField] private float sprintSpeed = 100f;
    [SerializeField] private float shieldSpeed = 50f;
    [SerializeField] private int speedScale = 1;
    public float jumpForce = 3;
    [SerializeField] private float doubleJumpForce = 2;
    private bool canTakeDamage = true;
    
    [Space]

    [Header("Death and such")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private  Image blackDeathScreen;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject hitTextPopUp;
    [SerializeField] private Image hitIndicator;
    [SerializeField] private float deathFadeInTime = 3f;
    [SerializeField] private float hitWearOffTime = 1f;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Material deathMaterial;
    [SerializeField] private SkinnedMeshRenderer playerOverall;
    [SerializeField] private SkinnedMeshRenderer playerDetails;
    private bool dead;
    private PlayerCombat combat;
    private SceneController SceneController;
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public PageController PageController;
    private ButtonManager buttonManager;

    #endregion

    #region Unity Functions
    private void Start() {
        combat = GetComponent<PlayerCombat>();
        controller = GetComponent<PlayerController>();
        currentHealth = maxHealth;

        ChangeSpeed(sprintSpeed);
        
        if(healthBar != null) {
            healthBar.maxValue = maxHealth;
        }

        SceneController = SceneController.instance;
        PageController = PageController.instance;
        if(SceneController) buttonManager = PageController.GetComponent<ButtonManager>();

        UpdateUI();
    }

    private void Update() {
        if(canAutoRegen)
        {
            ResetInCombat();
            
            if(outOfCombatTimer == 0)
            {
                RegenHealth();
            }
        }

        if(SceneController != null) {
            if(currentHealth <= 0) {
                if(!dead) {
                    Die("death");
                }
            }
        }
    }
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int amount)
    {
        if ((currentHealth + amount) > maxHealth)
        {
            currentHealth = maxHealth;
        } else {
            currentHealth += amount;
        }

        UpdateUI();
    }
    public void RegenHealth()
    {
        if(currentHealth == 0 || currentHealth == maxHealth) return;

        float count = 0;
        if (currentHealth > 0) { count += Time.deltaTime; }
        
        while (count > 0)
        {
            if ((currentHealth + autoRegenAmount) > maxHealth)
            {
                currentHealth = maxHealth;
            } else {
                currentHealth += autoRegenAmount;
            }
            count = 0;

            UpdateUI();
        }
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(float amount)
    {
        if(!canTakeDamage) return;

        if ((currentHealth - amount) < 0)
        {
            currentHealth = 0;
        } else {
            currentHealth -= amount;
        }

        PlayerEnteredCombat();

        ShowFloatingText(amount.ToString());
        HitUI();


        UpdateUI();
    }

    #endregion
    #region Stat Reporters]
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }
    public float GetSprintSpeed()
    {
        return sprintSpeed;
    }
    public float GetCrouchSpeed()
    {
        return crouchSpeed;
    }
    public float GetShieldSpeed()
    {
        return shieldSpeed;
    }
    public float GetDoubleJumpForce()
    {
        return doubleJumpForce;
    }
    #endregion
    #region Custom Functions
    public void ParryDecision(int amount, Transform enemy)
    {
        if(combat.canParry)
        {
            StartCoroutine(enemy.GetComponent<EnemyStats>().Confusion());
        } else {
            TakeDamage(amount);
        }
    }
    public void PlayerEnteredCombat()
    {
        outOfCombatTimer = outOfCombatResetTime;
    }
    public void ResetInCombat()
    {
        if(outOfCombatTimer == 0) return;

        float count = 0;
        if (outOfCombatTimer > 0) { count += Time.deltaTime; }
        
        while (count > 0)
        {
            if ((outOfCombatTimer - outOfCombatRate) < 0)
            {
                outOfCombatTimer = 0;
            } else {
                outOfCombatTimer -= outOfCombatRate;
            }
            count = 0;
        }
    }
    public void Die(string animationTrigger) {
        dead = true;
        LightsGoOut();
        DisableComponents();
        GetComponent<Animator>().SetTrigger(animationTrigger);
        playerOverall.material = deathMaterial;
        playerDetails.material = deathMaterial;
        StartCoroutine(FadeToBlackScreen());
    }
    private void LightsGoOut() {
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.cullingMask = whatIsPlayer;
    }
    private void DisableComponents() {
        GameUI.SetActive(false);
        Camera.main.GetComponent<CameraController>().enabled = false;
        GetComponent<PlayerPhysics>().enabled = false;
        controller.enabled = false;
        combat.enabled = false;
        combat.currentAbilities.gameObject.SetActive(false);
    }
    private IEnumerator FadeToBlackScreen() {
        yield return new WaitForSeconds(2);
        
        blackDeathScreen.gameObject.SetActive(true);
        StartCoroutine(LerpAlpha(deathFadeInTime, blackDeathScreen.color.a, 1f, 1));
    }
    private void ShowFloatingText(string amount) {
        var hitText = Instantiate(hitTextPopUp, transform.position, Quaternion.identity);
        hitText.GetComponent<TextMesh>().text = amount;
    }
    private void HitUI() 
    {
        hitIndicator.color = new Color(hitIndicator.color.r, hitIndicator.color.b, hitIndicator.color.g, 0.4f);
        StartCoroutine(LerpAlpha(hitWearOffTime, hitIndicator.color.a, 0f, 0));

    }
    private IEnumerator LerpAlpha(float time, float startValue, float targetValue, int colorNumber)
    {
        float start = Time.time;

        while (Time.time < start + time)
        {
            float completion = (Time.time - start) / time;

            // NOTE: 0 IS THE HIT INDICATOR, 1 IS END GAME BLACK SCREEN 
            switch (colorNumber)
            {
                case 0:
                    hitIndicator.color = new Color(hitIndicator.color.r, hitIndicator.color.b, hitIndicator.color.g, Mathf.Lerp(startValue, targetValue, completion));
                    break;
                
                case 1:
                    blackDeathScreen.color = new Color(blackDeathScreen.color.r, blackDeathScreen.color.b, blackDeathScreen.color.g, Mathf.Lerp(startValue, targetValue, completion));
                    break;
            }

            yield return null;
        }
        switch (colorNumber)
        {
            case 0:
                hitIndicator.color = new Color(hitIndicator.color.r, hitIndicator.color.b, hitIndicator.color.g, targetValue);
                break;
            
            case 1:
                blackDeathScreen.color = new Color(blackDeathScreen.color.r, blackDeathScreen.color.b, blackDeathScreen.color.g, targetValue);
                if(buttonManager) buttonManager.RestartFromCheckpoint();
                break;
        }
    }
    public void ChangeSpeed(float target) {
        currentSpeed = target * speedScale;
    }
    public void ChangeHealthState(bool state) {
        canTakeDamage = state;
    }
    private void UpdateUI() {
        if(healthBar != null) {
            healthBar.value = currentHealth;
            //healthText.text = currentHealth.ToString();
        }
    }

    #endregion
}

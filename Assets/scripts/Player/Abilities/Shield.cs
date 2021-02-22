using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shield : MonoBehaviour
{
    public float shieldMaxHealth;
    public float shieldCurrentHealth;
    public int autoRegenAmount;
    public int autoBreakAmount;
    public Slider shieldHp;
    public TextMeshProUGUI shieldHpText;
    public GameObject shield;
    public ShieldAbility ability;

    private void Awake() {
        shieldCurrentHealth = shieldMaxHealth;
    }

    private void Update() {
        if(shieldCurrentHealth <= 0) {
            shieldCurrentHealth = 0;
            ApplyHealth(0);
            ability.DisableShield();
        }


        if(!ability.blocking)
            RegenerateShield();
        else
            DecreaseShield();
    }

    private void RegenerateShield() {
        float count = 0;
        if (shieldCurrentHealth < shieldMaxHealth) { count += Time.deltaTime; }
        
        while (count > 0)
        {
            ApplyHealth(autoRegenAmount);
            count = 0;
        }
    }

    private void DecreaseShield() {
        float count = 0;
        if (shieldCurrentHealth > 0) { count += Time.deltaTime; }
        
        while (count > 0)
        {
            BlockDamage(autoRegenAmount);
            count = 0;
        }
    }
    
    public void ApplyHealth(int healthAmount)
    {
        if ((shieldCurrentHealth + healthAmount) > shieldMaxHealth)
        {
            shieldCurrentHealth = shieldMaxHealth;
        } else {
            shieldCurrentHealth += healthAmount;
        }

        if(shieldHp != null)
            shieldHp.value = shieldCurrentHealth;
        if(shieldHpText != null)
            shieldHpText.text = shieldCurrentHealth.ToString();
    }

    public void BlockDamage(int amount) {
        shieldCurrentHealth -= amount;

        if(shieldHp != null)
            shieldHp.value = shieldCurrentHealth;
        if(shieldHpText != null)
            shieldHpText.text = shieldCurrentHealth.ToString();
    }
}

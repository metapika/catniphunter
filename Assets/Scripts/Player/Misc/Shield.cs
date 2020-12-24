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
    public Slider shieldHp;
    public TextMeshProUGUI shieldHpText;
    public GameObject shield;
    private PlayerAbilities abilites;

    private void Awake() {
        abilites = transform.root.GetComponent<PlayerAbilities>();
        shieldCurrentHealth = shieldMaxHealth;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.M))
            BlockDamage(100);
        
        if(shieldCurrentHealth <= 0)
            shield.SetActive(false);
        if(!abilites.blocking)
            RegenerateShield();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Bullet")) {
            BlockDamage(13);
            Destroy(other.gameObject);
        }
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

    private void BlockDamage(int amount) {
        shieldCurrentHealth -= amount;

        if(shieldHp != null)
            shieldHp.value = shieldCurrentHealth;
        if(shieldHpText != null)
            shieldHpText.text = shieldCurrentHealth.ToString();
    }
}

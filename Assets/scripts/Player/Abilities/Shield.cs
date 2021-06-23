using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shield : MonoBehaviour
{
    public ShieldAbility ability;
    public GameObject gfx;

    public void BlockDamage(int amount) {
        // transform.root.GetComponent<PlayerStats>().TakeDamage(0);
        // Debug.Log("Blocked " + amount / 4 + " damage!");
    }

    // public void ApplyHealth(int healthAmount)
    // {
    //     if ((shieldCurrentHealth + healthAmount) > shieldMaxHealth)
    //     {
    //         shieldCurrentHealth = shieldMaxHealth;
    //     } else {
    //         shieldCurrentHealth += healthAmount;
    //     }
    //
    //     if(shieldHp != null)
    //         shieldHp.value = shieldCurrentHealth;
    //     if(shieldHpText != null)
    //         shieldHpText.text = shieldCurrentHealth.ToString();
    // }
    // private void Update() {
    //     if(!ability.blocking)
    //         RegenerateShield();
    //     else
    //         DecreaseShield();
    // }
    // private void RegenerateShield() {
    //     float count = 0;
    //     if (shieldCurrentHealth < shieldMaxHealth) { count += Time.deltaTime; }
    //
    //     while (count > 0)
    //     {
    //         ApplyHealth(autoRegenAmount);
    //         count = 0;
    //     }
    // }
    // private void DecreaseShield() {
    //     float count = 0;
    //     if (shieldCurrentHealth > 0) { count += Time.deltaTime; }
    //
    //     while (count > 0)
    //     {
    //         BlockDamage(autoRegenAmount);
    //         count = 0;
    //     }
    // }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStats : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    public CharacterStats_SO characterDefinition;

    #region Initializations
    void Start()
    {
        characterDefinition.currentHealth = characterDefinition.maxHealth;
    }
    #endregion

    #region Private functions
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("HealthPack")) {
            Destroy(other.gameObject);
            if(GetComponent<PlayerAbilities>() != null)
                GetComponent<PlayerAbilities>().shield.GetComponent<Shield>().ApplyHealth(100);
            ApplyHealth(140);
        }
    }

    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        characterDefinition.ApplyHealth(healthAmount);
        if(healthBar != null)
            healthBar.value = characterDefinition.currentHealth;
        if(healthText != null)
            healthText.text = characterDefinition.currentHealth.ToString();
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        characterDefinition.TakeDamage(amount);
        if(healthBar != null)
            healthBar.value = characterDefinition.currentHealth;
        if(healthText != null)
            healthText.text = characterDefinition.currentHealth.ToString();
    }
    #endregion

    #region Reporters
    public int GetHealthValue()
    {
        return characterDefinition.currentHealth;
    }

    public int GetDamageValue()
    {
        return characterDefinition.baseDamage;
    }
    #endregion
}
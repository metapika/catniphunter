using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterStats_SO characterDefinition;

    #region Initializations
    void Start()
    {
        if (!characterDefinition.setManually)
        {
            characterDefinition.maxHealth = 100;
            characterDefinition.currentHealth = 50;

            characterDefinition.baseDamage = 0;
            characterDefinition.currentDamage = characterDefinition.baseDamage;

            characterDefinition.baseResistance = 0;
            characterDefinition.currentResistance = 0;
        }
    }
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        characterDefinition.ApplyHealth(healthAmount);
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        characterDefinition.TakeDamage(amount);
    }
    #endregion

    #region Reporters
    public int GetHealthValue()
    {
        return characterDefinition.currentHealth;
    }

    public int GetDamageValue()
    {
        return characterDefinition.currentDamage;
    }

    public float GetResistanceValue()
    {
        return characterDefinition.currentResistance;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStats.asset", menuName = "Character/Stats", order = 1)]
public class CharacterStats_SO : ScriptableObject
{
    #region Fields
    public bool setManually = false;

    public int maxHealth = 0;
    public int currentHealth = 0;

    public int baseDamage = 0;
    public int currentDamage = 0;

    public float baseResistance = 0f;
    public float currentResistance = 0f;

    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        if ((currentHealth+healthAmount) > maxHealth)
        {
            currentHealth = maxHealth;
        } else {
            currentHealth += healthAmount;
        }
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            //Make Death()
            //Death();
        }
    }
    #endregion

    #region Death
    private void Death()
    {
        Debug.Log("You loose");
        //Call gamemanager for death state to trigger respawn menu
        //Visualization
    }
    #endregion
}

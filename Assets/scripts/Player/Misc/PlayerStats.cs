using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    #region Fields
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    public int maxHealth = 0;
    public int currentHealth = 0;
    private Animator anim;

    #endregion

    #region Unity Functions
    private void Awake() {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        UpdateUI();

        anim = GetComponent<Animator>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            TakeDamage(30);
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
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        if ((currentHealth - amount) < 0)
        {
            currentHealth = 0;
        } else {
            currentHealth -= amount;
        }

        UpdateUI();
    }

    #endregion

    #region Custom Functions
    private void UpdateUI() {
        healthBar.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    #endregion
}

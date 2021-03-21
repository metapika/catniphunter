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
    public int currentHealth = 0;
    public int maxHealth = 0;
    public float currentSpeed;
    public float crouchSpeed = 30f;
    public float sprintSpeed = 100f;
    public int speedScale = 1;
    public float jumpForce = 3;
    public float doubleJumpForce = 2;
    [Space]

    [Header("Spectacular Death")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    public GameObject hitTextPopUp;
    public GameObject GameUI;
    public LayerMask whatIsPlayer;
    public Material deathMaterial;
    public SkinnedMeshRenderer playerOverall;
    public SkinnedMeshRenderer playerDetails;
    private bool dead;
    private SceneController SceneController;
    private PageController PageController;

    #endregion

    #region Unity Functions
    private void Awake() {
        //Health and speed values
        currentHealth = maxHealth;
        if(healthBar != null) {
            healthBar.maxValue = maxHealth;
        }
        currentSpeed = sprintSpeed * speedScale;

        //Death
        SceneController = SceneController.instance;
        PageController = PageController.instance;

        UpdateUI();
    }

    private void Update() {
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

        ShowFloatingText(amount.ToString());
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

        ShowFloatingText(amount.ToString());
        UpdateUI();
    }

    #endregion

    #region Custom Functions
    public void Die(string animationTrigger) {
        dead = true;
        LightsGoOut();
        DisableComponents();
        GetComponent<Animator>().SetTrigger(animationTrigger);
        playerOverall.material = deathMaterial;
        playerDetails.material = deathMaterial;
        StartCoroutine(ShowEndScreen());
    }
    private void LightsGoOut() {
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.cullingMask = whatIsPlayer;
    }
    private void DisableComponents() {
        GameUI.SetActive(false);
        Camera.main.GetComponent<CameraController>().enabled = false;
        GetComponent<PlayerPhysics>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<PlayerCombat>().currentAbilities.gameObject.SetActive(false);
    }
    private IEnumerator ShowEndScreen() {
        yield return new WaitForSeconds(1);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PageController.TurnPageOn(PageType.DeathScreen);
    }

    private void ShowFloatingText(string amount) {
        var hitText = Instantiate(hitTextPopUp, transform.position, Quaternion.identity);
        hitText.GetComponent<TextMesh>().text = amount;
    }

    public void ChangeSpeed(float target) {
        currentSpeed = target * speedScale;
    }

    private void UpdateUI() {
        if(healthBar != null) {
            healthBar.value = currentHealth;
            healthText.text = currentHealth.ToString();
        }
    }

    #endregion
}

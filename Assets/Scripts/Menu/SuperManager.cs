using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperManager : MonoBehaviour
{
    public GameObject controls;
    public GameObject deathScreen;
    public GameObject player;
    public CharacterStats playerStats;

    private void Awake() {
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controls.SetActive(false);
        deathScreen.SetActive(false);
        
        player = GameObject.Find("RoboSamurai");
        playerStats = player.GetComponent<CharacterStats>();

        //Lower usage
        Application.targetFrameRate = 300;
    }
    private void Update() {
        //Input
        if(Input.GetKeyDown(KeyCode.F5)) {
            Quit();
        }
        
        if(Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }

        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) {
            if(deathScreen.activeSelf == false) {
                if(controls.activeSelf == true) {
                    controls.SetActive(false);
                } else {
                    controls.SetActive(true);
                }
            }
        }
        
        //Check player health
        int hp = CheckPlayerHealth();
        if(hp <= 0) {
            KillPlayer();
        }
    }

    public void KillPlayer() {
        Time.timeScale = 0f;
        Destroy(player);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        deathScreen.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            KillPlayer();
        }
    }

    public int CheckPlayerHealth() {
        int playerHealth = playerStats.GetHealthValue();

        return playerHealth;
    }

    public void Restart() {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void Quit() {
        Application.Quit();
    }
}

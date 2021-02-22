using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityCore.Scene;
using UnityCore.Menu;

public class PauseMenu : MonoBehaviour
{
    private GameObject Player;
    private SceneController SceneController;
    
    private PageController PageController;

    void Start()
    {
        PageController = PageController.instance;
        SceneController = SceneController.instance;
    }

    void Update()
    {
        if(SceneController.currentSceneName != "Menu") {
            if(Player == null) {
                Player = GameObject.Find("RoboSamurai");
            }

            if(Input.GetButtonDown("Pause"))
            {
                TogglePauseMenu();
            }
        }
    }

    public void TogglePauseMenu()
    {
        if(!PageController.PageIsOn(PageType.PauseMenu))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            DisableComponents();
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EnableComponents();
        }
    }

    public void EnableComponents() {
        PageController.TurnPageOff(PageType.PauseMenu);
        Time.timeScale = 1f;
        if(Camera.main.GetComponent<CameraController>() != null)
            Camera.main.GetComponent<CameraController>().enabled = true;
        if(Player.GetComponent<PlayerController>() != null)
            Player.GetComponent<PlayerController>().enabled = true;
        if(Player.GetComponent<PlayerCombat>() != null)
            Player.GetComponent<PlayerCombat>().enabled = true;
        if(Player.transform.Find("CurrentAbilities").gameObject != null)
            Player.transform.Find("CurrentAbilities").gameObject.SetActive(true);
    }
    private void DisableComponents() {
        PageController.TurnPageOn(PageType.PauseMenu);
        Time.timeScale = 0f;
        if(Camera.main != null)
            Camera.main.GetComponent<CameraController>().enabled = false;
        if(Player.GetComponent<PlayerController>() != null)
            Player.GetComponent<PlayerController>().enabled = false;
        if(Player.GetComponent<PlayerCombat>() != null)
            Player.GetComponent<PlayerCombat>().enabled = false;
        if(Player.transform.Find("CurrentAbilities").gameObject != null)
            Player.transform.Find("CurrentAbilities").gameObject.SetActive(false);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityCore.Scene;

namespace UnityCore {
    
    namespace Menu {
        public class ButtonManager : MonoBehaviour
        {
            private PageController PageController;
            private SceneController SceneController;
            public PauseMenu PauseMenu;
            public GameObject menuOnly;

            void Start() {
                PageController = PageController.instance;
                SceneController = SceneController.instance;
                PauseMenu = GetComponent<PauseMenu>();
            }
            private void Awake() {
                CheckpointManagement.checkpointIndex = 0;
                PlayerCombat.weaponInventory.Clear();
            }

            public void ModeSelect() {
                PageController.TurnPageOff(PageType.MainMenu, PageType.ModeSelect);
            }
            public void MissionSelect() {
                PageController.TurnPageOff(PageType.ModeSelect, PageType.MissionSelect);
            }
            public void SaveSelect() {
                PageController.TurnPageOff(PageType.ModeSelect, PageType.SaveSelect);
            }
            public void CharacterCustomizer() {
                menuOnly.SetActive(false);
                PageController.TurnPageOff(PageType.MainMenu, PageType.Customize);
            }

            public void RestartHard() {
                PauseMenu.EnableComponents();
                PageController.TurnPageOff(PageType.DeathScreen);
                PageController.TurnPageOff(PageType.EndGameScreen);
                PauseMenu.gameEnded = false;
                
                CheckpointManagement.checkpointIndex = 0;
                PlayerCombat.weaponInventory.Clear();

                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
            public void RestartFromCheckpoint() {
                PauseMenu.EnableComponents();
                PageController.TurnPageOff(PageType.DeathScreen);
                PageController.TurnPageOff(PageType.EndGameScreen);

                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
            public void MainMenu()
            {
                PauseMenu.EnableComponents();
                PageController.TurnPageOff(PageType.DeathScreen);
                PageController.TurnPageOff(PageType.PauseMenu);
                PageController.TurnPageOff(PageType.EndGameScreen, PageType.MainMenu);
                //make the "are you sure" screen
                SceneController.Load(SceneType.Menus, (_scene) => {
                                     Debug.Log("Scene [" + _scene + "] loaded from the button manager!" );
                                    }, false, PageType.Loading);
            }
            public void LoadIntoHub() {
                PageController.TurnPageOff(PageType.ModeSelect);
                PageController.TurnPageOff(PageType.EndGameScreen);
                Time.timeScale = 1f;
                SceneController.Load(SceneType.Hub, (_scene) => {
                                     Debug.Log("Scene [" + _scene + "] loaded from the button manager!" );
                                    }, false, PageType.Loading);
            }
            public void Save()
            {
                if(SceneManager.GetActiveScene().name != "Menu")
                {
                    SavePlayer();
                }
            }
            public void SavePlayer() {
                SaveSystem.SavePlayer(GameObject.Find("RoboSamurai").GetComponent<PlayerStats>());
            }
            public void Quit() {
                Application.Quit();
            }
            // ------------------------Missions-----------------------------
            public void TutorialMission() {
                PageController.TurnPageOff(PageType.MissionSelect);
                PageController.TurnPageOff(PageType.ModeSelect);
                SceneController.Load(SceneType.TutorialLevel, (_scene) => {
                                     Debug.Log("Scene [" + _scene + "] loaded from the button manager!" );
                                    }, false, PageType.Loading);
            }
        }
    }
}

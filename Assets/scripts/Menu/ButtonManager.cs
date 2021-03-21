using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityCore.Scene;

namespace UnityCore {
    
    namespace Menu {
        public class ButtonManager : MonoBehaviour
        {
            private PageController PageController;
            public SceneController SceneController;
            public PauseMenu PauseMenu;
            public GameObject menuOnly;

            void Start()
            {
                PageController = GetComponent<PageController>();
            }

            public void ModeSelect() {
                PageController.TurnPageOff(PageType.MainMenu, PageType.ModeSelect);
            }
            public void SaveSelect() {
                PageController.TurnPageOff(PageType.ModeSelect, PageType.SaveSelect);
            }
            public void CharacterCustomizer() {
                menuOnly.SetActive(false);
                PageController.TurnPageOff(PageType.MainMenu, PageType.Customize);
            }
            public void LoadIntoGame() {
                PageController.TurnPageOff(PageType.SaveSelect);
                SceneController.Load(SceneType.TestZone, (_scene) => {
                                     Debug.Log("Scene [" + _scene + "] loaded from the button manager!" );
                                    }, false, PageType.Loading);
            }
            public void Restart() {
                PauseMenu.EnableComponents();
                if(PageController.PageIsOn(PageType.DeathScreen))
                    PageController.TurnPageOff(PageType.DeathScreen);
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
            public void MainMenu()
            {
                PauseMenu.EnableComponents();
                PageController.TurnPageOff(PageType.DeathScreen);
                PageController.TurnPageOff(PageType.PauseMenu, PageType.MainMenu);
                //make the "are you sure" screen
                SceneController.Load(SceneType.Menu, (_scene) => {
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

        }
    }
}

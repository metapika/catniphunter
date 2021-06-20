using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Menu;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            PlayerStats player = other.GetComponent<PlayerStats>();

            if(player.PageController) {
                PageController.instance.GetComponent<PauseMenu>().gameEnded = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                player.PageController.TurnPageOn(PageType.EndGameScreen);
            } else Debug.Log("You won! Tho the Menu managers arent in the scene so uhh, the only reward you get is satisfaction. Nice!");
        }
    }
}

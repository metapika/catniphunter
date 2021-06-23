using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public float health;
    public int damage;

    public PlayerData (PlayerStats player) {
        health = player.currentHealth;
        //GET CURRENT WEAPON HERE
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStats : MonoBehaviour
{
    private void Start() {
        LoadPlayer();
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        PlayerStats stats = GameObject.Find("RoboSamurai").GetComponent<PlayerStats>();

        stats.TakeDamage(stats.currentHealth - data.health);
        //DO CURRENT WEAPON HERE
    }
}

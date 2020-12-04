using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // private int maxHealth = 100;
    // int currentHealth = 100;
    // void Start()
    // {
    //     currentHealth = maxHealth;   
    // }

    // public void TakeDamage(int damage)
    // {
    //     currentHealth -= damage;

    //     //play hurt

    //     if(currentHealth <= 0)
    //     {
    //         Die();
    //     }
    // }

    // private void Die()
    // {
    //     Debug.Log("Enemy Died!");
    //     //Die animation

    //     //Disable the enemy
    // }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Melee")) {
            Debug.Log(" Enemy got hit! ");
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int maxHealth = 60;
    int currentHealth = 60;

    private MeshRenderer my_renderer;
    public Material def;
    public Material hitMat;

    void Start()
    {
        my_renderer = GetComponent<MeshRenderer>();
        currentHealth = maxHealth;   
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Melee")) {
            if(my_renderer != null) {
                my_renderer.material = hitMat;
            }
            Invoke("DefaultMaterial", 0.5f);
            TakeDamage(20);
        }
    }
    
    private void DefaultMaterial() {
        my_renderer.material = def;
    }
}
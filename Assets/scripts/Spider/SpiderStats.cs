using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStats : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth = 0;
    public GameObject hitText;
    public int minDamage = 12;
    public int maxDamage = 15;
    public int currentDamage = 0;
    public bool confused;
    public float confusionTime = 10f;
    public ParticleSystem confusionParticles;

    private Transform player;
    void Awake() {
        currentHealth = maxHealth;

        player = GameObject.Find("RoboSamurai").transform;
    }

    void Update()
    {
        RandomizeDamage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Shuriken"))
        {
            //StartCoroutine(Confusion());
        }

        if(other.CompareTag("Melee")) {
            TakeDamage(20);
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(hitText != null)
            ShowFloatingText(amount.ToString());
    }
    private void ShowFloatingText(string amount) {
        Instantiate(hitText, transform.position, Quaternion.identity);
        hitText.GetComponent<TextMesh>().text = amount;
    }
    public void RandomizeDamage() {
        currentDamage = Random.Range(minDamage, maxDamage);
    }
    private void Die()
    {
        player.GetComponent<PlayerCombat>().targets.Remove(this.gameObject.transform);
        gameObject.SetActive(false);
    }
}

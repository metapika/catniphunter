using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStats : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth = 0;
    public GameObject hitText;
    public int damage = 5;
    public bool gettingKnockbacked = false;
    public float knockbackTime = 0.3f;
    public float confusionTime = 10f;
    public ParticleSystem confusionParticles;

    private Transform player;
    void Awake() {
        currentHealth = maxHealth;

        player = GameObject.Find("RoboSamurai").transform;
    }

    void Update()
    {
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
    private void Die()
    {
        player.GetComponent<PlayerCombat>().targets.Remove(this.gameObject.transform);
        gameObject.SetActive(false);
    }
    public IEnumerator Knockback(Transform pos, float knockbackStrenght)
    {
        gettingKnockbacked = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        rb.isKinematic = false;
        agent.enabled = false;

        yield return new WaitForSeconds(knockbackTime);

        rb.isKinematic = true;
        agent.enabled = true;

        gettingKnockbacked = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SpiderStats : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth = 0;
    public bool dead = false;
    public GameObject hitText;
    public bool gettingKnockbacked = false;
    public float knockbackTime = 0.3f;
    public float confusionTime = 1f;
    public bool confused = false;
    public LegReferences legs;
    public Material disintegrationMaterial;
    public List<Transform> materialModels = new List<Transform>();
    float materialAmount;
    public Transform playerVar;
    private NavMeshAgent agent;
    private SpiderShoot shoot;
    private SpiderMovement movement;
    private void Update() {
        if(dead) {
            if(playerVar) {
                playerVar.GetComponent<PlayerCombat>().targets.Remove(this.gameObject.transform);
            }
            materialAmount += Time.deltaTime / 2;
            disintegrationMaterial.SetFloat("amount", materialAmount);
            if(materialAmount > 1) {
                Destroy(gameObject);
            }
        }
    }
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        shoot = GetComponent<SpiderShoot>();
        movement = GetComponent<SpiderMovement>();
        
        currentHealth = maxHealth;
    }
    public void TakeDamage(int amount, Transform player = null)
    {
        if(dead) return;

        if(player && !playerVar) {
            playerVar = player;
        }
        currentHealth -= amount;

        if(currentHealth <= 0)
            StartCoroutine(Die());
        if(hitText != null)
            ShowFloatingText(amount.ToString());
        
    }
    public IEnumerator Confusion()
    {
        if(shoot) {
            confused = true;
            shoot.enabled = false;
            movement.enabled = false;
        }

        yield return new WaitForSeconds(confusionTime);

        if(shoot) {
            confused = false;
            shoot.enabled = true;
            movement.enabled = true;
        }
    }
    private void ShowFloatingText(string amount) {
        Instantiate(hitText, transform.position, Quaternion.identity);
        hitText.GetComponent<TextMesh>().text = amount;
    }
    private IEnumerator Die()
    {
        dead = true;

        agent.enabled = false;
        GetComponent<KeepBodyUp>().enabled = false;
        GetComponent<SpiderMovement>().enabled = false;
        GetComponent<SpiderShoot>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;

        foreach(LegMovement leg in legs.legParts)
        {
            leg.enabled = false;
        }
        foreach(Transform model in materialModels)
        {
            if(model.GetComponent<MeshRenderer>())
                model.GetComponent<MeshRenderer>().material = disintegrationMaterial;
            else
                model.GetComponent<SkinnedMeshRenderer>().material = disintegrationMaterial;
        }
        
        yield return null;
    }
    public IEnumerator Knockback(Transform pos, float knockbackStrenght)
    {
        // Rigidbody rb = GetComponent<Rigidbody>();
        // NavMeshAgent agent = GetComponent<NavMeshAgent>();
        
        // if(!dead) {
        //     gettingKnockbacked = true;

        //     rb.isKinematic = false;
        //     agent.enabled = false;
        // }

        float oldSpeed = agent.speed;
        Vector3 direction = transform.position - pos.root.position;
        direction.y = 0;

        agent.speed = 10f;
        if(agent.enabled) {
            agent.SetDestination(direction * knockbackStrenght);
        }

        yield return new WaitForSeconds(knockbackTime);
        
        agent.speed = oldSpeed;
        if(agent.enabled) {
            agent.SetDestination(transform.position);
        }

        // if(!dead) {
        //     rb.isKinematic = true;
        //     agent.enabled = true;

        //     gettingKnockbacked = false;
        // }
    }
}

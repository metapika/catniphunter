using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyStats : MonoBehaviour
{
    [Header("--------------------------Universal--------------------------")]
    public int maxHealth = 50;
    public int currentHealth = 0;
    public bool dead = false;
    private NavMeshAgent agent;
    public GameObject hitText;
    public bool gettingKnockbacked = false;
    public float confusionTime = 1f;
    public bool confused = false;
    public Transform playerVar;
    private PlayerCombat playerCombat;
    public Material disintegrationMaterial;
    public List<Transform> materialModels = new List<Transform>();
    float materialAmount;

    [Space]

    [Header("--------------------------Spider--------------------------")]
    public LegReferences legs;
    private KeepBodyUp kbUp;
    private EnemyShoot shoot;
    private EnemySight sight;
    private EnemyMovement movement;
    private Rigidbody rb;
    private void Update() {
        if(dead) {
            materialAmount += Time.deltaTime / 2;
            disintegrationMaterial.SetFloat("amount", materialAmount);
            if(materialAmount > 1) {
                Destroy(gameObject);
            }
        }
    }
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        shoot = GetComponent<EnemyShoot>();
        movement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody>();
        kbUp = GetComponent<KeepBodyUp>();
        playerCombat = playerVar.GetComponent<PlayerCombat>();
        sight = GetComponent<EnemySight>();

        currentHealth = maxHealth;
    }
    public void TakeDamage(int amount, bool stealthKill, Transform player = null)
    {
        if(dead) return;

        if(player) {
            playerVar = player;
            sight.player = player;
        }

        if(!stealthKill) { sight.justSawPlayer = true; sight.AlertOthers(); }

        currentHealth -= amount;

        if(currentHealth <= 0)
            StartCoroutine(Die(stealthKill));
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
    private IEnumerator Die(bool stealthKill)
    {
        dead = true;

        //Alert nearby
        if(!stealthKill)
        {
            sight.AlertOthers();
        }

        gameObject.tag = "Untagged";
        if(playerCombat) playerCombat.enemyDetector.RemoveEnemy(transform, true);
        
        agent.enabled = false;

        if(kbUp) kbUp.enabled = false;

        if(movement) movement.enabled = false;
        if(shoot) shoot.enabled = false;
        rb.isKinematic = false;
        rb.useGravity = true;

        if(legs)
        {
            foreach(LegMovement leg in legs.legParts)
            {
                leg.enabled = false;
            }
        }
        if(materialModels.Count > 0) {
            foreach(Transform model in materialModels)
            {
                if(model.GetComponent<MeshRenderer>())
                    model.GetComponent<MeshRenderer>().material = disintegrationMaterial;
                else
                    model.GetComponent<SkinnedMeshRenderer>().material = disintegrationMaterial;
            }
        }
        
        yield return null;
    }

    // public IEnumerator Knockback(Transform pos, float knockbackStrenght)
    // {
    //     // Rigidbody rb = GetComponent<Rigidbody>();
    //     // NavMeshAgent agent = GetComponent<NavMeshAgent>();
        
    //     // if(!dead) {
    //     //     gettingKnockbacked = true;

    //     //     rb.isKinematic = false;
    //     //     agent.enabled = false;
    //     // }

    //     float oldSpeed = agent.speed;
    //     Vector3 direction = transform.position - pos.root.position;
    //     direction.y = 0;

    //     agent.speed = 10f;
    //     if(agent.enabled) {
    //         agent.SetDestination(direction * knockbackStrenght);
    //     }

    //     yield return new WaitForSeconds(knockbackTime);
        
    //     agent.speed = oldSpeed;
    //     if(agent.enabled) {
    //         agent.SetDestination(transform.position);
    //     }

    //     // if(!dead) {
    //     //     rb.isKinematic = true;
    //     //     agent.enabled = true;

    //     //     gettingKnockbacked = false;
    //     // }
    // }
}

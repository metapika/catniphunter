using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject projectile;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform gunPoint;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public float invokeNextWalkPoint;
    
    //Guns
    public int magazineSize;
    public float timeBetweenAttacks, reloadTime;
    bool alreadyAttacked, reloading;
    int bulletsShot;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerNotBehindCover;
    public bool justSawPlayer;
    private Animator anim;
    private float IKweight;

    //Health system
    public int maxHealth = 50;
    public int currentHealth = 0;

    public int baseDamage = 12;

    private void Awake() {
        player = GameObject.Find("RoboSamurai").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    private void Update() {
        //Check sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(player != null) {
            RaycastHit hit;
            Vector3 direction = player.transform.position - transform.position;     
            
            Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, player.position));
            
            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Player")) {
                    justSawPlayer = true;
                    playerNotBehindCover = true;
                }
                else
                    playerNotBehindCover = false;
            }
        }

        if(!reloading && bulletsShot == magazineSize)
            StartCoroutine(Reload());

        if(!playerInSightRange && !playerInAttackRange) Patroling();
        
        if(playerInAttackRange && playerInSightRange && playerNotBehindCover) AttackPlayer();
        else if(playerInSightRange && justSawPlayer) ChasePlayer();
    }

    private void Patroling()
    {
        IKweight = 0f;
        agent.SetDestination(transform.position);
        // if(!walkPointSet) SearchWalkPoint();
        
        // anim.SetBool("patroling", true);
        // anim.SetBool("aim", false); 
        // anim.SetBool("chasePlayer", false);

        // if(walkPointSet)
        //     agent.SetDestination(walkPoint);
        
        // Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // if(distanceToWalkPoint. magnitude < 1f)
        //     walkPointSet = false;
    }

    private void SearchWalkPoint() {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        
        anim.SetBool("chasePlayer", true);
        anim.SetBool("patroling", false);
        anim.SetBool("aim", false); 
    }
    
    private void AttackPlayer()
    {
        IKweight = 0.1f;
        //Stop enemy
        agent.SetDestination(transform.position);
        
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        gunPoint.LookAt(player);

        anim.SetBool("chasePlayer", false);
        anim.SetBool("patroling", false);
        anim.SetBool("aim", true);

        if(bulletsShot < magazineSize) {
            if(!alreadyAttacked) {
                bulletsShot++;

                //Attack code here depending on enemy type
                BulletBase bullet = Instantiate(projectile, gunPoint.position, Quaternion.identity).GetComponent<BulletBase>();
                bullet.gunPoint = gunPoint;
                
                alreadyAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }
    }

    private void OnAnimatorIK(int layerIndex) {
        if(player != null)
        {            
            anim.SetIKPosition(AvatarIKGoal.RightHand, player.position);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, player.position);
            anim.SetLookAtPosition(player.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, IKweight);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKweight);
            anim.SetLookAtWeight(IKweight * 5f);
        }
    }

    private IEnumerator Reload() {
        IKweight = 0f;
        reloading = true;
        anim.SetBool("reloading", true);
        
        yield return new WaitForSeconds(reloadTime);

        IKweight = 0.1f;
        reloading = false;
        anim.SetBool("reloading", false);
        bulletsShot = 0;
        alreadyAttacked = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Melee")) {
            anim.SetTrigger("hit_ranged");
            
            TakeDamage(player.gameObject.GetComponent<CharacterStats>().characterDefinition.baseDamage);
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        alreadyAttacked = false;
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        player.GetComponent<PlayerCombat>().targets.Remove(this.gameObject.transform);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

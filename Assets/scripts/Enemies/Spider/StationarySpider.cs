using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StationarySpider : MonoBehaviour
{
    [HideInInspector] public enum EnemyType {
        BurstBlaster,
        HeavyMissile
    }
    public EnemyType enemyType;
    public Transform player;
    [Space]
    public GameObject blasterProjectile;
    public GameObject missileProjectile;
    public Transform rifle, rifle2, gunPoint, gunPoint2;
    public GameObject shootingParticles;
    [Space]
    public int damage = 5;
    public int burstShots = 3;
    public float timeBetweenBursts = 0.2f;
    public float timeBetweenAttacks = 1.1f;
    public float attackRange;
    [Space]
    public LayerMask whatIsPlayer;
    public LayerMask coverDetectionMask;
    public bool canShoot = true;
    public bool playerInAttackRange, playerNotBehindCover;

    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        Vector3 direction = player.transform.position - transform.position;     
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, player.position) + 5f, coverDetectionMask))
        {
            if(hit.transform.gameObject.CompareTag("Player") || hit.transform.gameObject.CompareTag("Shield")) 
            {
                playerNotBehindCover = true;
            }
            else
            {
                playerNotBehindCover = false;
            }
        }

        if(playerInAttackRange) {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        if(playerInAttackRange && playerNotBehindCover) AttackPlayer();
    }
    private void AttackPlayer()
    {
        rifle.LookAt(PredictPosition(player));
        rifle2.LookAt(PredictPosition(player));
        gunPoint.LookAt(PredictPosition(player));
        gunPoint2.LookAt(PredictPosition(player));

        if(enemyType == EnemyType.BurstBlaster) {
            if(canShoot) {
                StartCoroutine(FireBurst(blasterProjectile, burstShots));
            }
        } else if(enemyType == EnemyType.HeavyMissile) {
            if(canShoot) {
                StartCoroutine(FireBurst(missileProjectile, burstShots));
            }
        }
    }
    private Vector3 PredictPosition(Transform target){
        Vector3 pos = target.position;
        Vector3 dir = target.GetComponent<PlayerPhysics>().controllerVelocity;
        
        float dist = (pos-transform.position).magnitude;

        return pos + (dist/blasterProjectile.GetComponent<BulletBase>().bulletSpeed)*dir;
    }
    public IEnumerator FireBurst(GameObject bulletPrefab, int burstSize)
    {
        canShoot = false;
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.
        for (int i = 0; i < burstSize + 1; i++)
        {
            Instantiate(shootingParticles, gunPoint.position, gunPoint.rotation);
            Instantiate(shootingParticles, gunPoint2.position, gunPoint2.rotation);

            BulletBase bullet1 = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation).GetComponent<BulletBase>();
            BulletBase bullet2 = Instantiate(bulletPrefab, gunPoint2.position, gunPoint2.rotation).GetComponent<BulletBase>();
                
            bullet1.bulletDamage = damage;
            bullet2.bulletDamage = damage;
            yield return new WaitForSeconds(timeBetweenBursts);
        }
        
        yield return new WaitForSeconds(timeBetweenAttacks); // wait till the next round
        
        canShoot = true;
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

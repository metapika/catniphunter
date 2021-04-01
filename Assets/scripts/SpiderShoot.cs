using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderShoot : MonoBehaviour
{
    [HideInInspector] public enum EnemyType {
        BurstBlaster,
        HeavyMissile
    }
    public EnemyType enemyType;
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
    [Space]
    public bool canShoot = true;

    private SpiderSight sight;
    private SpiderStats stats;
    
    void Start()
    {
        sight = GetComponent<SpiderSight>();
        stats = GetComponent<SpiderStats>();
    }

    void Update()
    {
        if(!stats.gettingKnockbacked)
        {
            if(sight.playerInAttackRange) AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        rifle.LookAt(PredictPosition(sight.player));
        rifle2.LookAt(PredictPosition(sight.player));
        gunPoint.LookAt(PredictPosition(sight.player));
        gunPoint2.LookAt(PredictPosition(sight.player));

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
}

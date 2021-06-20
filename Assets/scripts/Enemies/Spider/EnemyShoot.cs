using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [HideInInspector] public enum EnemyType {
        BurstBlaster,
        HeavyMissile
    }
    [Header("--------------------------Universal--------------------------")]
    public EnemyType enemyType;
    public GameObject blasterProjectile;
    public string blasterProjectileTag;
    public GameObject missileProjectile;
    public Transform rifle, rifle2, gunPoint, gunPoint2;
    public string shootingParticlesTag;
    [Space]
    public bool canShoot = true;
    public int damage = 5;
    public int burstShots = 3;
    public float timeBetweenBursts = 0.2f;
    public float timeBetweenAttacks = 1.1f;

    private EnemySight sight;
    private EnemyStats stats;
    private ObjectPooler objectPooler;
    
    void Start()
    {
        sight = GetComponent<EnemySight>();
        stats = GetComponent<EnemyStats>();
        objectPooler = ObjectPooler.instance;
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
        Vector3 pos = Vector3.zero;
        Vector3 dir = Vector3.zero;
        float dist = 0;

        if(target != null && target.GetComponent<PlayerPhysics>())
        {
            pos = target.position;
            dir = target.GetComponent<PlayerPhysics>().controllerVelocity;
            dist = (pos-transform.position).magnitude;
        }

        return pos + (dist/blasterProjectile.GetComponent<BulletBase>().bulletSpeed)*dir;
    }
    public IEnumerator FireBurst(GameObject bulletPrefab, int burstSize)
    {
        canShoot = false;
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.
        for (int i = 0; i < burstSize + 1; i++)
        {
            objectPooler.SpawnFromPool(shootingParticlesTag, gunPoint.position, gunPoint.rotation);
            objectPooler.SpawnFromPool(shootingParticlesTag, gunPoint2.position, gunPoint2.rotation);

            BulletBase bullet1 = objectPooler.SpawnFromPool(blasterProjectileTag, gunPoint.position, gunPoint.rotation).GetComponent<BulletBase>();
            BulletBase bullet2 = objectPooler.SpawnFromPool(blasterProjectileTag, gunPoint2.position, gunPoint2.rotation).GetComponent<BulletBase>();
                
            bullet1.bulletDamage = damage;
            bullet2.bulletDamage = damage;

            bullet1.enemy = transform;
            bullet2.enemy = transform;

            yield return new WaitForSeconds(timeBetweenBursts);
        }
        
        yield return new WaitForSeconds(timeBetweenAttacks); // wait till the next round
        
        canShoot = true;
    }
}
